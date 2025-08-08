using UnityEngine;

public class CameraRayHitDetector : IHitDetector
{
    private readonly Camera _camera;
    private readonly float _maxDistance;

    public CameraRayHitDetector(Camera cam, float maxDistance = 12f)
    {
        _camera = cam;
        _maxDistance = maxDistance;
    }

    public bool TryHit(LayerMask mask, Vector2 screenPos, out RaycastHit hit)
    {
        var ray = _camera.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out hit, _maxDistance, mask);
    }
}
