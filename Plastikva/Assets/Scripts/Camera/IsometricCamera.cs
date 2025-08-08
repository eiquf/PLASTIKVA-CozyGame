using System;
using UnityEngine;
using Zenject;

public class IsometricCamera : MonoBehaviour, ICameraContext
{
    [Inject] private readonly CameraInputHandler _input;
    private bool _initialized = false;

    public Camera Camera { get; private set; }

    [field: SerializeField] public float DragSpeed { get; private set; } = 0.5f;
    private bool _isDragging;

    [field: Header("Zoom")]
    [field: SerializeField] public float MinZoom { get; private set; } = 5f;
    [field: SerializeField] public float MaxZoom { get; private set; } = 10f;
    [field: SerializeField] public float ZoomSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ZoomSmoothnes { get; private set; } = 5f;

    public float RotationSpeed { get; private set; } = 1f;
    public float SnapSpeed { get; private set; } = 6f;
    public float MinRot { get; private set; } = 0f;
    public float MaxRot { get; private set; } = 90f;
    public bool IsRotating { get; private set; }

    private Transform _followTarget;
    private bool _followEnabled;
    [field: SerializeField] public float FollowSmoothSpeed { get; private set; } = 5f;

    private Vector3 _lastTargetPosition;

    private BoxCollider _boundaryCollider;

    #region Functions
    private ICamera<BoxCollider> _boundaries;
    private ICamera<Vector2> _drag;
    private ICamera<Transform> _follow;
    private ICamera<Vector2> _rotate;
    private ICamera<Vector2> _zoom;
    #endregion

    public void Initialize()
    {
        _input.OnLeftMouseClick += HandleLeftClick;
        _input.OnRightMouseClick += HandleRightClick;

        Camera = Camera.main;
        Camera.orthographicSize = MaxZoom;

        Init();

        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (_initialized)
        {
            CheckTargetMovement();

            Vector2 delta = _input.Delta();
            Vector2 deltaScroll = _input.DeltaScroll();

            if (_isDragging && !_followEnabled)
                _drag.Execute(transform, delta);

            if (_followEnabled && _followTarget != null)
                _follow.Execute(transform, _followTarget);

            _rotate.Execute(transform, delta);

            _boundaries.Execute(transform, _boundaryCollider);
            _zoom?.Execute(transform, deltaScroll);
        }
    }
    private void HandleLeftClick(bool isPressed) => _isDragging = isPressed;
    private void HandleRightClick(bool isPressed) => IsRotating = isPressed;

    private void Init()
    {
        _drag = new CameraDrag(this);
        _follow = new CameraFollowTarget(this);
        _rotate = new CameraRotate(this);
        _zoom = new CameraZoom(this);
        _boundaries = new CameraBoundries();
    }
    public void SetBoundraries(BoxCollider collider) => _boundaryCollider = collider;
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
        _followEnabled = target != null;
    }
    private void CheckTargetMovement()
    {
        if (_followTarget == null) return;

        float movedDistance = Vector3.Distance(_followTarget.position, _lastTargetPosition);

        _followEnabled = movedDistance > 0.001f;

        _lastTargetPosition = _followTarget.position;
    }
    private void OnDestroy()
    {
        _input.OnLeftMouseClick -= HandleLeftClick;
        _input.OnRightMouseClick -= HandleRightClick;
    }
}