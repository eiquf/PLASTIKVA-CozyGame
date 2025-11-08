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
    [field: SerializeField] public float MinZoom { get; private set; } = 5f;
    [field: SerializeField] public float MaxZoom { get; private set; } = 10f;
    [field: SerializeField] public float ZoomSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ZoomSmoothness { get; private set; } = 5f;

    public float RotationSpeed { get; private set; } = 1f;
    public float SnapSpeed { get; private set; } = 6f;
    public float MinRot { get; private set; } = -20f;
    public float MaxRot { get; private set; } = 110f;

    private bool _isDragging;
    public bool IsRotating { get; private set; }

    private bool _followEnabled;
    [field: SerializeField] public float FollowSmoothSpeed { get; private set; } = 5f;
    private Vector3 _lastTargetPosition;

    private Vector2 _rotationVelocity;
    private float _rotationDamp = 5f; 


    private ICamera<Vector2> _drag;
    private ICamera<Transform> _follow;
    private ICamera<Vector2> _rotate;
    private ICamera<Vector2> _zoom;

    private readonly ReactiveProperty<bool> _isBackSide = new();
    public Observable<bool> IsBackSide => _isBackSide;

    public void Initialize()
    {
        _input.OnLeftMouseClick += HandleLeftClick;
        _input.OnRightMouseClick += HandleRightClick;
        _input.OnZoom += HandleZoom;
        _input.OnRotate += HandleRotate;

        Camera = GetComponentInChildren<Camera>();
        Camera.orthographicSize = 10f;

        transform.rotation = Quaternion.Euler(0f, -20f, 0f);

        Init();

        _zoom?.Execute(transform, new Vector2(0, -1f));

        if (Target != null)
            _lastTargetPosition = Target.position;

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

        if (Target != null && _followEnabled)
            _follow.Execute(transform);

        if (IsRotating)
            _rotationVelocity = delta * RotationSpeed;
        else
            _rotationVelocity = Vector2.Lerp(_rotationVelocity, Vector2.zero, Time.fixedDeltaTime * _rotationDamp);

        if (_rotationVelocity.sqrMagnitude > 0.0001f)
            _rotate.Execute(transform, _rotationVelocity);

        _zoom?.Execute(transform, deltaScroll);

        float yaw = Camera.transform.eulerAngles.y;
        bool backSide = yaw > 90f && yaw < 270f;
        _isBackSide.Value = backSide;
    }


    private void HandleLeftClick(bool isPressed)
    {
        _isDragging = isPressed;
        _followEnabled = false;
    }
    private void HandleRightClick(bool isPressed) => IsRotating = isPressed;

    private void HandleZoom(float zoomDelta)
    {
        float newSize = Mathf.Clamp(Camera.orthographicSize + zoomDelta * ZoomSpeed, MinZoom, MaxZoom);
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, newSize, Time.deltaTime * ZoomSmoothness);
    }
    private void HandleRotate(Vector2 delta)
    {
        if (IsRotating && Application.isMobilePlatform)
            _rotate.Execute(transform, delta * RotationSpeed);
    }

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
        _followEnabled = true;
        _lastTargetPosition = Target.position;
    }

    private void CheckTargetMovement()
    {
        if (Target == null) return;
        if (Target.position != _lastTargetPosition) _followEnabled = true;
        _lastTargetPosition = Target.position;

    }

    private void OnDestroy()
    {
        _input.OnLeftMouseClick -= HandleLeftClick;
        _input.OnRightMouseClick -= HandleRightClick;
        _input.OnZoom -= HandleZoom;
        _input.OnRotate -= HandleRotate;
    }
}
