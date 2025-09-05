using UnityEngine;

public class CameraRayHitDetector : IHitDetector
{
    private readonly Camera _camera;
    private readonly float _maxDistance = Mathf.Infinity;

    private readonly LayerMask DefaultMask = 1 << 0;

    public CameraRayHitDetector(Camera camera) => _camera = camera;

    public bool TryHit(LayerMask layerMask, Vector2 input, bool destroy)
    {
        if (!_camera) return false;

        Vector3 mousePos = input;
        Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePos);

        int mask = layerMask | DefaultMask;

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, _maxDistance, mask);

        if (hit.collider != null)
        {
            int hitLayerBit = 1 << hit.collider.gameObject.layer;

            if ((hitLayerBit & DefaultMask) != 0) return false;

            if ((hitLayerBit & mask) != 0)
            {
                if (destroy)
                    Object.Destroy(hit.collider.gameObject);

                return true;
            }
        }

        return false;
    }

    public GameObject TryGetHitObject(LayerMask layerMask, Vector2 input)
    {
        if (!_camera) return null;

        Vector3 mousePos = input;
        Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePos);

        int mask = layerMask | DefaultMask;

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, _maxDistance, mask);

        if (hit.collider != null)
        {
            int hitLayerBit = 1 << hit.collider.gameObject.layer;

            if ((hitLayerBit & DefaultMask) != 0) return null;

            if ((hitLayerBit & mask) != 0)
                return hit.collider.gameObject;
        }

        return null;
    }
}
