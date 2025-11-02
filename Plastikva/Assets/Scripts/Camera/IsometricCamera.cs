using R3;
using UnityEngine;
using Zenject;

public class IsometricCamera : MonoBehaviour, ICameraContext
{
    [Inject] private readonly CameraInputHandler _input;
    private bool _initialized = false;

    public Camera Camera { get; private set; }
    public Transform Target { get; private set; }
    [field: SerializeField] public float DragSpeed { get; private set; } = 0.5f;
    private bool _isDragging;

    [field: Header("Zoom")]
    [field: SerializeField] public float MinZoom { get; private set; } = 5f;
    [field: SerializeField] public float MaxZoom { get; private set; } = 10f;
    [field: SerializeField] public float ZoomSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ZoomSmoothness { get; private set; } = 5f;

    public float RotationSpeed { get; private set; } = 1f;
    public float SnapSpeed { get; private set; } = 6f;
    public float MinRot { get; private set; } = -20f;
    public float MaxRot { get; private set; } = 110f;
    public bool IsRotating { get; private set; }

    private bool _followEnabled;
    [field: SerializeField] public float FollowSmoothSpeed { get; private set; } = 5f;

    private Vector3 _lastTargetPosition;

    #region Functions
    private ICamera<Vector2> _drag;
    private ICamera<Transform> _follow;
    private ICamera<Vector2> _rotate;
    private ICamera<Vector2> _zoom;
    #endregion

    private readonly ReactiveProperty<bool> _isBackSide = new();
    public Observable<bool> IsBackSide => _isBackSide;

    public void Initialize()
    {
        _input.OnLeftMouseClick += HandleLeftClick;
        _input.OnRightMouseClick += HandleRightClick;

        Camera = GetComponentInChildren<Camera>();
        Camera.orthographicSize = 10f;

        Init();

        _zoom?.Execute(transform, new Vector2(0, -1f));
        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (!_initialized) return;

        CheckTargetMovement();

        Vector2 delta = _input.Delta();
        Vector2 deltaScroll = _input.DeltaScroll();

        if (_isDragging && !_followEnabled)
            _drag.Execute(transform, delta);

        _follow.Execute(transform);

        _rotate.Execute(transform, delta);

        _zoom?.Execute(transform, deltaScroll);

        float yaw = Camera.transform.eulerAngles.y;
        bool backSide = yaw > 90f && yaw < 270f;
        _isBackSide.Value = backSide;
    }

    private void HandleLeftClick(bool isPressed) => _isDragging = isPressed;
    private void HandleRightClick(bool isPressed) => IsRotating = isPressed;
    private void Init()
    {
        _drag = new CameraDrag(this);
        _follow = new CameraFollowTarget(this);
        _rotate = new CameraRotate(this);
        _zoom = new CameraZoom(this);
    }
    public void SetFollowTarget(Transform target)
    {
        Target = target;
        _followEnabled = target != null;
        _lastTargetPosition = Target.position;
    }
    private void CheckTargetMovement()
    {
        if (Target == null) return;

        float movedDistance = Vector3.Distance(Target.position, _lastTargetPosition);

        _followEnabled = movedDistance > 0.001f;

        _lastTargetPosition = Target.position;
    }
    private void OnDestroy()
    {
        _input.OnLeftMouseClick -= HandleLeftClick;
        _input.OnRightMouseClick -= HandleRightClick;
    }
}