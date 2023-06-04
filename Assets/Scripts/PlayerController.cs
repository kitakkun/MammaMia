using System;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private float dashSpeed = 5f;

    private Vector2 _currentMoveInput = Vector2.zero;
    // non nullチェックがうまくいかないっぽいのでzeroが指定されている時をnull扱いする
    private Vector2 _currentGroundNormal = Vector2.zero;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _initialAngularDrag = _rigidbody2D.angularDrag;
    }

    void OnMove(InputValue inputValue)
    {
        var moveInput = inputValue.Get<Vector2>();
        _currentMoveInput = moveInput;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        UpdateGroundNormal(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        UpdateGroundNormal(other);
    }

    private void OnCollisionExit2D(Collision2D _)
    {
        _currentGroundNormal = Vector2.zero;
    }

    private void UpdateGroundNormal(Collision2D other)
    {
        var minY = other.contacts.Min(contact => contact.point.y);
        var minYCollider = other.contacts.First(contact => Math.Abs(contact.point.y - minY) < float.Epsilon);
        _currentGroundNormal = minYCollider.normal;
    }

    void OnDash(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (_currentGroundNormal == Vector2.zero) return;
        
        float radAngle = 90 * Mathf.Deg2Rad;
        radAngle *= -Mathf.Sign(_currentMoveInput.x);
        var dashDirection = new Vector2(
            x: _currentGroundNormal.x * Mathf.Cos(radAngle) - _currentGroundNormal.y * Mathf.Sin(radAngle),
            y: _currentGroundNormal.x * Mathf.Sin(radAngle) + _currentGroundNormal.y * Mathf.Cos(radAngle)
        );

        _rigidbody2D.velocity += dashDirection.normalized * dashSpeed;
    }
}