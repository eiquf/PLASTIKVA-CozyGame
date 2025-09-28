using UnityEngine;

public class CameraFollowTarget : ICamera<Transform>
{
    private readonly ICameraContext _context;
    public CameraFollowTarget(ICameraContext context) => _context = context;
    public void Execute(Transform transform, Transform target = null)
    {
        Vector3 targetPosition = new(
                _context.Target.position.x,
                transform.position.y,
                _context.Target.position.z
            );
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _context.FollowSmoothSpeed);
    }
}