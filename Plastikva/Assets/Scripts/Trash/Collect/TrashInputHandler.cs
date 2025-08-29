using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TrashInputHandler : IInitializable, IDisposable
{
    private readonly InputController _controller;

    public event Action LeftMouseClicked;
    public event Action RightMouseClicked;

    private InputAction _collectingAction;
    private InputAction _pointerPosAction;
    private InputAction _dragAction;

    [Inject]
    public TrashInputHandler(InputController controller)
    {
        _controller = controller;
    }

    public void Initialize()
    {
        var gameplay = _controller.Input.Gameplay;

        _collectingAction = gameplay.Collecting;
        _pointerPosAction = gameplay.PointerPos;
        _dragAction = gameplay.Drag;

        _collectingAction.performed += OnCollectingPerformed;
        _collectingAction.Enable();
    }

    public void Dispose()
    {
        _collectingAction.performed -= OnCollectingPerformed;
    }

    public Vector2 GetMousePosition()
    {
        return _pointerPosAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public Vector2 GetDrag()
    {
        return _dragAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    private void OnCollectingPerformed(InputAction.CallbackContext context)
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        if (context.control == mouse.leftButton)
            LeftMouseClicked?.Invoke();
        else if (context.control == mouse.rightButton)
            RightMouseClicked?.Invoke();
    }
}
