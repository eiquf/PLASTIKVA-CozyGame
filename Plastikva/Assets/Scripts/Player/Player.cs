using R3;
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

    public Transform Bubbles { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public IPlayerState SwimState { get; private set; }
    public IPlayerState IdleState { get; private set; }

    public bool IsFacingRight {  get; set; }
    #endregion

    private PlayerInputHandler _inputHandler;
    private ISaveService _save;
    private PlayerAnim _animSystem;
    private IsometricCamera _camera;

    private readonly CompositeDisposable _disposables = new();

    private Transform _shield;

    [Inject]
    public void Container(ISaveService save, PlayerInputHandler inputHandler)
    {
        _save = save;
        _inputHandler = inputHandler;
    }
    public void Initialize(IsometricCamera camera)
    {
        Rigidbody = GetComponent<Rigidbody>();
        Bubbles = transform.GetChild(0);
        Animator = GetComponent<Animator>();
        _shield = transform.GetChild(1);

        _camera = camera;

        _animSystem = new PlayerAnim(Animator);
        StateMachine = new PlayerStateMachine();
        StatesInit();

        _camera.IsBackSide.Skip(1).Subscribe(OnCameraSideChanged).AddTo(_disposables);

        StateMachine.SetState(SwimState);

        _inputHandler.OnSprintChanged += HandleSprintChanged;
        _inputHandler.OnMoveInputChanged += HandleMoveChanged;

        transform.position = _save.Data.playerPos;

        _initialized = true;
    }

    void FixedUpdate()
    {
        if (_initialized)
        {
            _save.Data.playerPos = transform.position;
            StateMachine.Tick();
        }
    }
    private void StatesInit()
    {
        SwimState = new SwimMovementState(this, _animSystem);
        IdleState = new IdleState(this, _animSystem);
    }
    private void HandleSprintChanged(bool isSprinting) => IsSprinting = isSprinting;
    private void HandleMoveChanged(Vector2 vector) => CurrentInput = vector;
    private void OnCameraSideChanged(bool backSide)
    {
        if (_shield == null) return;

        var local = _shield.localPosition;
        local.z = backSide ? -Mathf.Abs(local.z) : Mathf.Abs(local.z);
        _shield.localPosition = local;
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
        _inputHandler.OnSprintChanged -= HandleSprintChanged;
        _inputHandler.OnMoveInputChanged -= HandleMoveChanged;
    }
}
