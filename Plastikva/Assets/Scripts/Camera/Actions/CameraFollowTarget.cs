using UnityEngine;

public class CameraFollowTarget : ICamera<Transform>
{
    private readonly ICameraContext _context;
    public CameraFollowTarget(ICameraContext context) => _context = context;
    public void Execute(Transform transform, Transform target)
    {
        Vector3 targetPosition = new(
                target.position.x,
                transform.position.y,
                target.position.z
            );
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _context.FollowSmoothSpeed);
    }
}