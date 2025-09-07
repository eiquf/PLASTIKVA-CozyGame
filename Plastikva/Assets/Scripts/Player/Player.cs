using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Player : MonoBehaviour, IPlayerContext
{
    private bool _initialized = false;
    public Vector2 CurrentInput { get; private set; }
    public bool IsSprinting { get; private set; }

    public float SlowSpeed { get; private set; } = 10f;
    public float AccelerationSpeed { get; private set; } = 15f;

    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public BoxCollider Boundry { get; private set; }

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public IPlayerState SwimState { get; private set; }
    public IPlayerState IdleState { get; private set; }

    public SpriteRenderer Renderer { get; private set; }

    [field:SerializeField] public Sprite[] Sprites { get; private set; }
    #endregion

    [Inject] private readonly PlayerInputHandler _inputHandler;
    public void Initialize()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Renderer = GetComponent<SpriteRenderer>();

        StateMachine = new PlayerStateMachine();
        StatesInit();

        StateMachine.SetState(SwimState);

        _inputHandler.OnSprintChanged += HandleSprintChanged;
        _inputHandler.OnMoveInputChanged += HandleMoveChanged;

        _initialized = true;
    }

    void FixedUpdate()
    {
        if (_initialized)
            StateMachine.Tick();
    }
    private void StatesInit()
    {
        SwimState = new SwimMovementState(this);
        IdleState = new IdleState(this);
    }
    private void HandleSprintChanged(bool isSprinting) => IsSprinting = isSprinting;
    private void HandleMoveChanged(Vector2 vector) => CurrentInput = vector;

    private void OnDestroy()
    {
        _inputHandler.OnSprintChanged -= HandleSprintChanged;
        _inputHandler.OnMoveInputChanged -= HandleMoveChanged;
    }
}