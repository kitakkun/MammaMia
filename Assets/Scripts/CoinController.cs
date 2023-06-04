using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private ParticleSystem getEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(
            original: getEffect,
            position: gameObject.transform.position,
            rotation: Quaternion.identity
        );
        Destroy(gameObject);
    }
}