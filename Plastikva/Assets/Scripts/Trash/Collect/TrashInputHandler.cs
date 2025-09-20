using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


public class TrashInputHandler : IInitializable, IDisposable
{
    private readonly InputController _controller;

    public event Action LeftMouseClicked;

    private InputAction _collectingAction;
    private InputAction _pointerPosAction;
    private InputAction _dragAction;

    [Inject]
    public TrashInputHandler(InputController controller) => _controller = controller;

    public void Initialize()
    {
        var gameplay = _controller.Input.Gameplay;

        _collectingAction = gameplay.Collecting;
        _pointerPosAction = gameplay.PointerPos;
        _dragAction = gameplay.Drag;


        _collectingAction.performed += _ => LeftMouseClicked?.Invoke();
        _collectingAction.Enable();
    }

    public void Dispose() => _collectingAction.performed -= _ => LeftMouseClicked?.Invoke();

    public Vector2 GetMousePosition() => _pointerPosAction?.ReadValue<Vector2>() ?? Vector2.zero;

    public Vector2 GetDrag() => _dragAction?.ReadValue<Vector2>() ?? Vector2.zero;
}
