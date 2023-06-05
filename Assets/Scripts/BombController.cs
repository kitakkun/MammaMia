using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private float timeToBomb = 5f;
    [SerializeField] private float explodeRadius = 2f;
    [SerializeField] private float explodePower = 5f;
    [SerializeField] private ParticleSystem explodeEffect;

    private SpriteRenderer[] _renderers;
    private SpriteRenderer[] _childRenderers;
    private ParticleSystem _sparkParticleSystem;
    private bool _isAlreadyTriggered;

    void Start()
    {
        _renderers = GetComponents<SpriteRenderer>();
        _childRenderers = GetComponentsInChildren<SpriteRenderer>();
        _sparkParticleSystem = GetComponentInChildren<ParticleSystem>();
        _isAlreadyTriggered = false;
        _sparkParticleSystem.Stop();
    }

    private void OnCollisionEnter2D(Collision2D _)
    {
        if (_isAlreadyTriggered) return;
        _isAlreadyTriggered = true;
        _sparkParticleSystem.Play();
        Invoke(nameof(Explode), timeToBomb);
    }

    private void Explode()
    {
        DisableSelfRendering();
        MakeExplosionEffect();
        BlowOffNearObjects();
        Destroy(gameObject);
    }

    private void MakeExplosionEffect()
    {
        Instantiate(
            original: explodeEffect,
            position: transform.position,
            rotation: Quaternion.identity
        );
    }

    private void DisableSelfRendering()
    {
        foreach (var spriteRenderer in _renderers)
        {
            spriteRenderer.enabled = false;
        }

        foreach (var spriteRenderer in _childRenderers)
        {
            spriteRenderer.enabled = false;
        }

        _sparkParticleSystem.Stop();
    }

    private void BlowOffNearObjects()
    {
        var selfPosition = transform.position;
        // ReSharper disable once Unity.PreferNonAllocApi
        var hitColliders = Physics2D.OverlapCircleAll(selfPosition, explodeRadius);
        foreach (var hitCollider in hitColliders)
        {
            var targetRigidBody = hitCollider.gameObject.GetComponent<Rigidbody2D>();
            if (targetRigidBody == null) continue;
            var direction = (targetRigidBody.position - new Vector2(selfPosition.x, selfPosition.y)).normalized;
            targetRigidBody.AddForce(force: direction * explodePower, ForceMode2D.Impulse);
        }
    }
}