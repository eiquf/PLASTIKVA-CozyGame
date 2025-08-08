using UnityEngine;

public class PlayerMovement : IMovementBehaviour
{
    private readonly float _slowSpeed;
    private readonly float _accelerationSpeed;

    private float _speed;

    private Vector2 _moveDirection;
    private readonly Rigidbody _rb;

    public PlayerMovement(IPlayerContext context)
    {
        _rb = context.Rigidbody;
        _slowSpeed = context.SlowSpeed;
        _accelerationSpeed = context.AccelerationSpeed;
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
    }
}
