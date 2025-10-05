using UnityEngine;

public interface IMovable
{
    bool MoveTo(Vector3 targetPos, Transform currentPos);
}
