using UnityEngine;

public class MovementAnimator : IMovable
{
    private readonly float _moveSpeed = 2f;

    public bool MoveTo(Vector3 targetPosition, Transform currentPos)
    {
        float distance = Vector3.Distance(currentPos.position, targetPosition);
        if (distance > 0.05f)
        {
            currentPos.position = Vector3.MoveTowards(
                currentPos.position,
                targetPosition,
                (_moveSpeed + Random.Range(-0.1f, 0.1f)) * Time.deltaTime
            );
            return false;
        }

        currentPos.position = targetPosition;
        return true; 
    }
}
