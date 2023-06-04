using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float _initialAngularDrag;
    [SerializeField] private float jumpPower = 400f;
    [SerializeField] private float moveFrictionFactor = 50f;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _initialAngularDrag = _rigidbody2D.angularDrag;
    }

    void OnMove(InputValue inputValue)
    {
        var moveInput = inputValue.Get<Vector2>();
        var movingDirection = _rigidbody2D.velocity.normalized;
        if (Vector3.Angle(from: movingDirection, to: moveInput) > 90f)
        {
            Debug.Log("Increase");
            IncreaseFriction();
        }
        else
        {
            Debug.Log("Decrease");
            DecreaseFriction();
        }
    }

    private void IncreaseFriction()
    {
        _rigidbody2D.angularDrag = _initialAngularDrag + moveFrictionFactor;
    }

    private void DecreaseFriction()
    {
        _rigidbody2D.angularDrag = _initialAngularDrag - moveFrictionFactor;
    }

    void OnJump(InputValue inputValue)
    {
        Debug.Log("JUMP");
        if (inputValue.isPressed)
        {
            _rigidbody2D.AddForce(Vector2.up * jumpPower);
        }
    }
}