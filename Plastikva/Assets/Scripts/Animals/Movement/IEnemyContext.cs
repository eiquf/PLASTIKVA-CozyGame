using UnityEngine;

public interface IEnemyContext
{
    bool MoveToPlayer { get; }
    Transform Pos { get; }
    /*Switch*/
    float SwitchDistance { get; }
    float SwitchDelay { get; }
    /*Chase Resume When Far*/
    float FarDistance { get; }
    float ReturnDelay { get; }
    /*Movement*/
    float Speed { get; }
    float ZigzagAmplitude { get; }
    float ZigzagFrequency { get; }
    float DesyncDelay { get; }
    LayerMask ObstacleMask { get; }
    /*Avoidance*/
    float AvoidDistance { get; }
    float SideProbeDistance { get; }
    float AvoidWeight { get; }
    float EnemyRadius { get; }
    float MaxSteerAngle { get; }

    Transform FollowTarget { get; }
}