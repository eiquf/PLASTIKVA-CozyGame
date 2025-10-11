using UnityEngine;

public interface IMovementBehavior
{
    void Execute(Vector2 input, bool isSprinting);
    bool FacingRight();
}
