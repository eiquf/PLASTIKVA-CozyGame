using UnityEngine;

public class CameraRotate : ICamera<Vector2>
{
    private readonly ICameraContext _context;
    private readonly float _minRot;
    private readonly float _maxRot;
    private float _currentRot;
    public CameraRotate(ICameraContext context)
    {
        _context = context;
        _minRot = context.MinRot;
        _maxRot = context.MaxRot;
    }
    public void Execute(Transform transform, Vector2 delta)
    {
        float deltaX = delta.x;

        if (_context.IsRotating)
        {
            float rotationAmount = deltaX * _context.RotationSpeed;
            _currentRot += rotationAmount;
            _currentRot = Mathf.Clamp(_currentRot, _minRot, _maxRot);
        }
        else
        {
            float targetRot = Mathf.Abs(_currentRot - _minRot) < Mathf.Abs(_currentRot - _maxRot) ? _minRot : _maxRot;
            _currentRot = Mathf.Lerp(_currentRot, targetRot, Time.deltaTime * _context.SnapSpeed);
        }

        transform.rotation = Quaternion.Euler(0, _currentRot, 0);
    }
}