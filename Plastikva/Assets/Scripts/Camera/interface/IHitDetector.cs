using UnityEngine;

public interface IHitDetector
{
    bool TryHit(LayerMask mask, Vector2 screenPos);
    GameObject TryGetHitObject(LayerMask mask, Vector2 screenPos);
}
