using UnityEngine;

public class CameraRayHitDetector : IHitDetector
{
    private readonly Camera _camera;

    public CameraRayHitDetector(Camera camera) => _camera = camera;

    public bool TryHit(LayerMask layerMask, Vector2 screenPos)
    {
        if (!_camera) return false;

        if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
            return false;

        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
        return hit.collider != null;
    }

    public GameObject TryGetHitObject(LayerMask layerMask, Vector2 input)
    {
        if (!_camera) return null;

        if (input.x < 0 || input.x > Screen.width || input.y < 0 || input.y > Screen.height)
        {
            Debug.LogWarning($"Invalid screen position: {input}");
            return null;
        }

        Ray ray = _camera.ScreenPointToRay(input);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
        return hit.collider ? hit.collider.gameObject : null;
    }
}