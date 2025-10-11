using UnityEngine;

public interface ICameraContext
{
    Transform Target { get; }
    Camera Camera { get; }
    bool IsRotating { get; }
    #region Параметры гспд
    float DragSpeed { get; }
    float FollowSmoothSpeed { get; }
    float ZoomSmoothness { get; }
    float ZoomSpeed { get; }

    float MinZoom { get; }
    float MaxZoom { get; }

    float RotationSpeed { get; }
    float SnapSpeed { get; }
    float MinRot { get; }
    float MaxRot { get; }
    #endregion
}