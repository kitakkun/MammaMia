using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private float timeToBomb = 5f;
    [SerializeField] private float explodeRadius = 2f;
    [SerializeField] private float explodePower = 5f;
    [SerializeField] private ParticleSystem explodeEffect;

    private SpriteRenderer[] _renderers;
    private bool _isAlreadyTriggered = false;

    void Start()
    {
        _renderers = GetComponents<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D _)
    {
        if (_isAlreadyTriggered) return;
        _isAlreadyTriggered = true;
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
    }

    private void BlowOffNearObjects()
    {
        var selfPosition = transform.position;
        // ReSharper disable once Unity.PreferNonAllocApi
        var hitColliders = Physics2D.OverlapCircleAll(selfPosition, explodeRadius);
        foreach (var hitCollider in hitColliders)
        {
            var targetRigidBody = hitCollider.gameObject.GetComponent<Rigidbody2D>();
            var direction = (targetRigidBody.position - new Vector2(selfPosition.x, selfPosition.y)).normalized;
            if (targetRigidBody == null) continue;
            targetRigidBody.AddForce(force: direction * explodePower, ForceMode2D.Impulse);
        }
    }
}