using UnityEngine;

public interface IHitDetector
{
    bool TryHit(LayerMask mask, Vector2 screenPos, bool destroy);
    GameObject TryGetHitObject(LayerMask mask, Vector2 screenPos);
}
