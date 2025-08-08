using UnityEngine;

public interface IPlayerContext
{
    #region Speed Parametres
    float SlowSpeed { get; }
    float AccelerationSpeed { get; }
    #endregion
    Vector2 CurrentInput { get; }
    bool IsSprinting { get; }
    Rigidbody Rigidbody { get; }
    Animator Animator { get; }

    #region State Machine
    PlayerStateMachine StateMachine { get; }
    //states
    IPlayerState SwimState { get; }
    IPlayerState IdleState { get; }
    #endregion
}
