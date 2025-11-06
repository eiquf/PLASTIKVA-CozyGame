using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TutorInputHandler : IInitializable, IDisposable
{
    private readonly InputController _controller;
    private readonly CameraInputHandler _cameraInput;

    private InputAction _collectingAction;
    private Action<InputAction.CallbackContext> _collectingPerformedHandler;

    public event Action LeftMouseClicked;
    public event Action CameraDragged;   
    public event Action CameraZoomed;    
    public event Action CameraRotated;   

    [Inject]
    public TutorInputHandler(InputController inputController, CameraInputHandler cameraInput)
    {
        _controller = inputController;
        _cameraInput = cameraInput;
    }

    public Vector2 MovementInput() => _controller.Input.Gameplay.Move.ReadValue<Vector2>();
    public Vector2 CameraInput() => _controller.Input.Camera.MouseDelta.ReadValue<Vector2>();

    public void Initialize()
    {
        var gameplay = _controller.Input.Gameplay;
        _collectingAction = gameplay.Collecting;

        _collectingPerformedHandler = _ => LeftMouseClicked?.Invoke();
        _collectingAction.performed += _collectingPerformedHandler;

        _cameraInput.OnRightMouseClick += OnRightClick;
    }

    public void Dispose()
    {
        if (_collectingAction != null && _collectingPerformedHandler != null)
            _collectingAction.performed -= _collectingPerformedHandler;

        _cameraInput.OnRightMouseClick -= OnRightClick;
    }

    private bool _isRightClickHeld;

    private void OnRightClick(bool isHeld)
    {
        _isRightClickHeld = isHeld;
    }

    public void Update()
    {
        if (Mouse.current == null) return;

        Vector2 delta = _cameraInput.Delta();
        if (_isRightClickHeld)
        {
            if (delta.sqrMagnitude > 0.01f)
                CameraDragged?.Invoke();
            CameraRotated?.Invoke();
        }

        Vector2 scroll = _cameraInput.DeltaScroll();
        if (Mathf.Abs(scroll.y) > 0.01f)
            CameraZoomed?.Invoke();

        //Vector3 rotate = _controller.Input.Camera.MouseDelta.ReadValue<Vector3>();

    }
}
