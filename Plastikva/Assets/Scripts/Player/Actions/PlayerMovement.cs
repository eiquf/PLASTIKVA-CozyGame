using UnityEngine;

public class PlayerMovement : IMovementBehaviour
{
    private readonly float _slowSpeed = 5f;
    private readonly float _accelerationSpeed = 10f;

    private float _speed;

    private Vector2 _moveDirection;
    private readonly Rigidbody _rb;

    public PlayerMovement(Rigidbody rb) => _rb = rb;
    public void Execute(Vector2 input, bool isSprinting)
    {
        _moveDirection = new Vector2(input.x, 0f).normalized;
        _speed = isSprinting ? _accelerationSpeed : _slowSpeed;

        Vector3 currentVelocity = new(_rb.velocity.x, 0f, _rb.velocity.z);
        float currentSpeed = currentVelocity.magnitude;

        float speedDifference = _speed - currentSpeed;
        float movementForce = speedDifference * _accelerationSpeed;

        _rb.AddForce(_moveDirection * movementForce, ForceMode.Acceleration);
    }
}
