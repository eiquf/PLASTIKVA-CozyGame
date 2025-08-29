using UnityEngine;

public class CameraBoundries : ICamera<BoxCollider>
{
    public void Execute(Transform transform, BoxCollider collider)
    {
        if (collider != null)
        {
            Vector3 cameraPos = transform.position;
            Vector3 closestPoint = collider.ClosestPoint(cameraPos);

            cameraPos.x = closestPoint.x;
            cameraPos.z = closestPoint.z;

            transform.position = cameraPos;
        }
    }
}