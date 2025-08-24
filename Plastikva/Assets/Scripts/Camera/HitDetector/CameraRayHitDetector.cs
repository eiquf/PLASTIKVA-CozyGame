using UnityEngine;

public class CameraRayHitDetector : IHitDetector
{
    private readonly Camera _camera;
    private readonly float _maxDistance = Mathf.Infinity;

    private readonly LayerMask DefaultMask = 1 << 0;

    public CameraRayHitDetector(Camera camera) => _camera = camera;
    public bool TryHit(LayerMask layerMask, Vector2 input)
    {
        if (!_camera) return false;

        Vector3 mousePos = input;
        Ray ray = _camera.ScreenPointToRay(mousePos);

        int mask = layerMask | DefaultMask;

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, mask, QueryTriggerInteraction.Ignore))
        {
            int hitLayerBit = 1 << hit.collider.gameObject.layer;

            if ((hitLayerBit & DefaultMask) != 0) return false;

            if ((hitLayerBit & mask) != 0)
            {
                Object.Destroy(hit.collider.gameObject);
                return true;
            }
        }

        return false;
    }
}
