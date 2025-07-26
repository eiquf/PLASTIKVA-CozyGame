using UnityEngine;

public class CameraDrag : ICamera<Vector2>
{
    private readonly ICameraContext _context;
    public CameraDrag(ICameraContext context) => _context = context;
    public void Execute(Transform transform, Vector2 input)
    {
        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        Vector3 move = (right * -input.x + forward * -input.y) * (_context.DragSpeed * Time.deltaTime);

        transform.position += move;
    }
}