using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputHandler : IInitializable, IDisposable
{
    private readonly InputController _inputController;

    private bool _wasSprinting;
    private Vector2 _lastInput;

    public event Action<bool> OnSprintChanged;
    public event Action<Vector2> OnMoveInputChanged;

    [Inject]
    public PlayerInputHandler(InputController inputController) => _inputController = inputController;

    public void Initialize()
    {
        _inputController.Input.Gameplay.Acceleration.performed += OnSprintStarted;
        _inputController.Input.Gameplay.Acceleration.canceled += OnSprintCanceled;

        _inputController.Input.Gameplay.Move.performed += OnMovePerformed;
        _inputController.Input.Gameplay.Move.canceled += OnMoveCanceled;
    }

    public void Dispose()
    {
        _inputController.Input.Gameplay.Acceleration.performed -= OnSprintStarted;
        _inputController.Input.Gameplay.Acceleration.canceled -= OnSprintCanceled;

        _inputController.Input.Gameplay.Move.performed -= OnMovePerformed;
        _inputController.Input.Gameplay.Move.canceled -= OnMoveCanceled;
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        if (_wasSprinting) return;

        _wasSprinting = true;
        OnSprintChanged?.Invoke(_wasSprinting);
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        if (!_wasSprinting) return;

        _wasSprinting = false;
        OnSprintChanged?.Invoke(_wasSprinting);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        if (input == _lastInput) return;

        _lastInput = input;
        OnMoveInputChanged?.Invoke(_lastInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _lastInput = Vector2.zero;
        OnMoveInputChanged?.Invoke(_lastInput);
    }

    public Vector2 CurrentInput() => _lastInput;
    public bool IsSprinting() => _wasSprinting;
}
