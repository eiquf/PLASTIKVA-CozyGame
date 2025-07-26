using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCamera : MonoBehaviour, ICameraContext
{
    private GameInput _input;
    public new Camera camera { get; private set; }

    [field: SerializeField] public float DragSpeed { get; private set; } = 0.5f;
    private bool _isDragging;

    [field: Header("Zoom")]
    [field: SerializeField] public float MinZoom { get; private set; } = 5f;
    [field: SerializeField] public float MaxZoom { get; private set; } = 10f;
    [field: SerializeField] public float ZoomSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ZoomSmoothnes { get; private set; } = 5f;

    [field: Header("Rotation")]
    [SerializeField] public float RotationSpeed { get; private set; } = 1f;
    [SerializeField] public float SnapSpeed { get; private set; } = 6f;
    [SerializeField] public float MinRot { get; private set; } = 0f;
    [SerializeField] public float MaxRot { get; private set; } = 90f;
    public bool IsRotating { get; private set; }

    [Header("Follow Target")]
    [SerializeField] private Transform _followTarget;
    private bool _followEnabled;
    [field: SerializeField] public float FollowSmoothSpeed { get; private set; } = 5f;

    private Vector3 _lastTargetPosition;

    [SerializeField] private BoxCollider _boundaryCollider;

    #region Functions
    private ICamera<BoxCollider> _boundaries;
    private ICamera<Vector2> _drag;
    private ICamera<Transform> _follow;
    private ICamera<Vector2> _rotate;
    private ICamera<Vector2> _zoom;
    #endregion

    private void Awake()
    {
        camera = Camera.main;
        _input = new GameInput();
        _input.Enable();

        _input.Camera.MouseClick.performed += OnMouseClick;
        _input.Camera.MouseClick.canceled += OnMouseClickEnd;

        SetFollowTarget(_followTarget);

        Init();
    }

    private void FixedUpdate()
    {
        CheckTargetMovement();

        Vector2 delta = _input.Camera.MouseDelta.ReadValue<Vector2>();
        Vector2 deltaScroll = _input.Camera.MouseScroll.ReadValue<Vector2>();

        if (_isDragging && !_followEnabled)
            _drag.Execute(transform, delta);

        if (_followEnabled && _followTarget != null)
            _follow.Execute(transform, _followTarget);

        _zoom.Execute(transform, deltaScroll);

        _rotate.Execute(transform, delta);

        _boundaries.Execute(transform, _boundaryCollider);
    }
    private void OnDisable()
    {
        _input.Camera.MouseClick.performed -= OnMouseClick;
        _input.Camera.MouseClick.canceled -= OnMouseClickEnd;
        _input.Disable();
    }
    private void Init()
    {
        _drag = new CameraDrag(this);
        _rotate = new CameraRotate(this);
        _zoom = new CameraZoom(this);
        _follow = new CameraFollowTarget(this);
        _boundaries = new CameraBoundries();
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        var control = context.control;

        if (control == Mouse.current.leftButton)
            _isDragging = true;
        else if (control == Mouse.current.rightButton)
            IsRotating = true;
    }
    private void OnMouseClickEnd(InputAction.CallbackContext context)
    {
        var control = context.control;

        if (control == Mouse.current.leftButton)
            _isDragging = false;
        else if (control == Mouse.current.rightButton)
            IsRotating = false;
    }
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
        _followEnabled = target != null;
    }
    private void CheckTargetMovement()
    {
        if (_followTarget == null) return;

        float movedDistance = Vector3.Distance(_followTarget.position, _lastTargetPosition);

        _followEnabled = movedDistance > 0.01f;

        _lastTargetPosition = _followTarget.position;
    }

}
