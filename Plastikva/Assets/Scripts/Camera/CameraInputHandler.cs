using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraInputHandler : IDisposable, IInitializable
{
    private readonly InputController _controller;

    public event Action<bool> OnLeftMouseClick;
    public event Action<bool> OnRightMouseClick;

    [Inject]
    public CameraInputHandler(InputController inputController) => _controller = inputController;

    public Vector2 Delta() => _controller.Input.Camera.MouseDelta.ReadValue<Vector2>();
    public Vector2 DeltaScroll() => _controller.Input.Camera.MouseScroll.ReadValue<Vector2>();

    public void Initialize()
    {
        _controller.Input.Camera.MouseClick.performed += OnMouseClick;
        _controller.Input.Camera.MouseClick.canceled += OnMouseClick;
    }

    public void Dispose()
    {
        _controller.Input.Camera.MouseClick.performed -= OnMouseClick;
        _controller.Input.Camera.MouseClick.canceled -= OnMouseClick;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        bool isDown = context.performed;
        var control = context.control;

        if (control == Mouse.current.leftButton)
            OnLeftMouseClick?.Invoke(isDown);
        else if (control == Mouse.current.rightButton)
            OnRightMouseClick?.Invoke(isDown);
    }
}