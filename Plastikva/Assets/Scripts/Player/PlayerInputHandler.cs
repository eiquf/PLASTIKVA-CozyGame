using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputHandler : IInitializable, IDisposable
{
    private readonly InputController _inputController;

    private bool _wasSprinting;
    private Vector2 _sprintDir = Vector2.zero;

    private Vector2 _lastInput;
    private bool _isMoving;

    private float _lastTapTime = -999f;
    private Vector2 _lastTapDir = Vector2.zero;

    private const float DoubleTapWindow = 0.30f;   
    private const float DirDotThreshold = 0.80f;   
    private const float TurnOffDot = 0.20f;        
    private const float MinTapMag = 0.50f;       

    public event Action<bool> OnSprintChanged;
    public event Action<Vector2> OnMoveInputChanged;

    [Inject]
    public PlayerInputHandler(InputController inputController)
    {
        _inputController = inputController;
    }

    public void Initialize()
    {
        var gameplay = _inputController.Input.Gameplay;

        gameplay.Move.performed += OnMovePerformed;
        gameplay.Move.canceled += OnMoveCanceled;
    }

    public void Dispose()
    {
        var gameplay = _inputController.Input.Gameplay;
        gameplay.Move.performed -= OnMovePerformed;
        gameplay.Move.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        OnMoveInputChanged?.Invoke(input);

        bool wasMoving = _isMoving;
        _isMoving = input.sqrMagnitude > 0.0001f;

        if (_wasSprinting && input.sqrMagnitude > 0.0001f)
        {
            var curDir = input.normalized;
            if (Vector2.Dot(curDir, _sprintDir) < TurnOffDot)
            {
                _wasSprinting = false;
                OnSprintChanged?.Invoke(false);
            }
        }

        if (!wasMoving && _isMoving)
        {
            var tapDir = input.magnitude >= MinTapMag ? input.normalized : Vector2.zero;

            if (tapDir != Vector2.zero)
            {
                if (Time.time - _lastTapTime <= DoubleTapWindow &&
                    Vector2.Dot(tapDir, _lastTapDir) >= DirDotThreshold)
                {
                    _wasSprinting = true;
                    _sprintDir = tapDir; // lock sprint direction
                    OnSprintChanged?.Invoke(true);
                }

                _lastTapTime = Time.time;
                _lastTapDir = tapDir;
            }
        }

        _lastInput = input;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _lastInput = Vector2.zero;
        _isMoving = false;
        OnMoveInputChanged?.Invoke(Vector2.zero);

        if (_wasSprinting)
        {
            _wasSprinting = false;
            OnSprintChanged?.Invoke(false);
        }
    }

    public Vector2 CurrentInput() => _lastInput;
    public bool IsSprinting() => _wasSprinting;
}
