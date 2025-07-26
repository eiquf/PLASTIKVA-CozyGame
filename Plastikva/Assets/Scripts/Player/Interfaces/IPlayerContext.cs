using UnityEngine;

public interface IPlayerContext
{
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
