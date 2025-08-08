using UnityEngine;

public interface ICameraContext
{
    Camera Camera { get; }
    bool IsRotating { get; }
    #region Параметры гспд
    float DragSpeed { get; }
    float FollowSmoothSpeed { get; }
    float ZoomSmoothnes { get; }
    float ZoomSpeed { get; }

    float MinZoom { get; }
    float MaxZoom { get; }

    float RotationSpeed { get; }
    float SnapSpeed { get; }
    float MinRot { get; }
    float MaxRot { get; }
    #endregion
}