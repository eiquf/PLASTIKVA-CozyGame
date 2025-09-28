using UnityEngine;

public class CameraDrag : ICamera<Vector2>
{
    private readonly ICameraContext _context;
    private Vector3 _targetPosition;
    private Vector3 _velocity;

    private readonly float _radius = 10f;

    public CameraDrag(ICameraContext context) => _context = context;

    public void Execute(Transform transform, Vector2 input)
    {
        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        Vector3 move = (right * -input.x + forward * -input.y)
                       * (_context.DragSpeed * Time.deltaTime);

        _targetPosition = transform.position + move;

        Vector3 newPos = Vector3.SmoothDamp(
            transform.position,
            _targetPosition,
            ref _velocity,
            0.1f);

        Vector3 offset = newPos - _context.Target.position;
        if (offset.sqrMagnitude > _radius * _radius)
        {
            offset = offset.normalized * _radius;
            newPos = _context.Target.position + offset;
        }

        transform.position = newPos;
    }
}
