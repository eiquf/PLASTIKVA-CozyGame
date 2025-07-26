using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Player : MonoBehaviour, IPlayerContext
{
    private GameInput _input;
    public Vector2 CurrentInput { get; private set; }
    public bool IsSprinting { get; private set; }

    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public IPlayerState SwimState { get; private set; }
    public IPlayerState IdleState { get; private set; }
    #endregion
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();

        StateMachine = new PlayerStateMachine();
        StatesInit();

        StateMachine.SetState(SwimState);
    }
    void FixedUpdate()
    {
        CurrentInput = _input.Gameplay.Move.ReadValue<Vector2>();
        StateMachine.Tick();
    }
    private void OnEnable()
    {
        _input = new GameInput();
        _input.Enable();

        _input.Gameplay.Acceleration.performed += OnAcceleration;
        _input.Gameplay.Acceleration.canceled += OnDeceleration;
    }
    private void OnDisable()
    {
        _input.Gameplay.Acceleration.performed -= OnAcceleration;
        _input.Gameplay.Acceleration.canceled -= OnDeceleration;
        _input.Disable();
    }
    private void StatesInit()
    {
        SwimState = new SwimMovementState(this);
        IdleState = new IdleState(this);
    }
    private void OnAcceleration(InputAction.CallbackContext ctx) => IsSprinting = true;
    private void OnDeceleration(InputAction.CallbackContext ctx) => IsSprinting = false;
}
