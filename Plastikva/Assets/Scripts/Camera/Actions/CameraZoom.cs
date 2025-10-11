using UnityEngine;

public class CameraZoom : ICamera<Vector2>
{
    private readonly Camera _camera;
    private readonly ICameraContext _context;
    private float _currentZoom;

    public CameraZoom(ICameraContext context)
    {
        _context = context;
        _camera = _context.Camera;
    }
    public void Execute(Transform transform, Vector2 deltaScroll)
    {
        if (deltaScroll.y != 0)
        {
            _currentZoom = Mathf.Clamp(
                _currentZoom - deltaScroll.y * _context.ZoomSpeed * Time.deltaTime,
                _context.MinZoom,
                _context.MaxZoom
            );
        }

        _camera.orthographicSize = Mathf.Lerp(
            _camera.orthographicSize,
            _currentZoom,
            _context.ZoomSmoothness * Time.deltaTime
        );
    }
}