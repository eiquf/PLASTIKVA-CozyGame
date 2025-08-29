using UnityEngine;

public interface IMovementBehaviour
{
    void Execute(Vector2 input, bool isSprinting);
}
