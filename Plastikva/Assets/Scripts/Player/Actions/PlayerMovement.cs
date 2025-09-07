using UnityEngine;

public class PlayerMovement : IMovementBehaviour
{
    private readonly float _slowSpeed;
    private readonly float _accelerationSpeed;

    private float _speed;
    private bool _facingForward = false;

    private Vector2 _moveDirection;
    private readonly Rigidbody _rb;
    private readonly SpriteRenderer _spriteRenderer;
    private Sprite[] _sprites;

    public PlayerMovement(IPlayerContext context)
    {
        _rb = context.Rigidbody;
        _slowSpeed = context.SlowSpeed;
        _accelerationSpeed = context.AccelerationSpeed;
        _spriteRenderer = context.Renderer;
        _sprites = context.Sprites;
    }
    public void Execute(Vector2 input, bool isSprinting)
    {
        _moveDirection = new Vector2(-input.x, 0f).normalized;

        _speed = isSprinting ? _accelerationSpeed : _slowSpeed;

        Vector3 currentVelocity = new(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        float currentSpeed = currentVelocity.magnitude;

        float speedDifference = _speed - currentSpeed;
        float movementForce = speedDifference * _accelerationSpeed;

        _rb.AddForce(_moveDirection * movementForce, ForceMode.Acceleration);

        if (_moveDirection.x > 0 && !_facingForward)
        {
            Flip();
            _spriteRenderer.sprite = _sprites[1];
        }
        else if (_moveDirection.x < 0 && _facingForward)
        {
            Flip();
            _spriteRenderer.sprite = _sprites[0];
        }
    }
    void Flip()
    {
        _facingForward = !_facingForward;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
