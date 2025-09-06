using UnityEngine;

public class CameraRayHitDetector : IHitDetector
{
    private readonly Camera _camera;
    private readonly float _maxDistance = Mathf.Infinity;

    private readonly LayerMask DefaultMask = 1 << 0;

    public CameraRayHitDetector(Camera camera) => _camera = camera;

    public bool TryHit(LayerMask layerMask, Vector2 screenPos)
    {
        if (!_camera) return false;

        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
        return hit.collider != null;
    }

    public GameObject TryGetHitObject(LayerMask layerMask, Vector2 input)
    {
        if (!_camera) return null;

        Ray ray = _camera.ScreenPointToRay(input);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
        return hit.collider ? hit.collider.gameObject : null;
    }
}
