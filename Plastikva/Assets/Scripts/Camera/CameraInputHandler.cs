using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraInputHandler : IDisposable, IInitializable
{
    private readonly InputController _controller;
    private readonly float _rightClickHoldLimit = 0.2f;
    private Coroutine _rightClickTimeoutCoroutine;
    private bool _rightClickBlocked;

    private readonly MonoBehaviour _coroutineHost;

    public event Action<bool> OnLeftMouseClick;
    public event Action<bool> OnRightMouseClick;

    [Inject]
    public CameraInputHandler(InputController inputController, [Inject(Id = "CoroutineHost")] MonoBehaviour coroutineHost)
    {
        _controller = inputController;
        _coroutineHost = coroutineHost;
    }

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
        {
            if (_rightClickBlocked)
                return;

            if (isDown)
            {
                OnRightMouseClick?.Invoke(true);

                if (_rightClickTimeoutCoroutine != null)
                    _coroutineHost.StopCoroutine(_rightClickTimeoutCoroutine);
                _rightClickTimeoutCoroutine = _coroutineHost.StartCoroutine(RightClickTimeout());
            }
            else
            {
                OnRightMouseClick?.Invoke(false);
                StopTimeout();
            }
        }
    }

    private IEnumerator RightClickTimeout()
    {
        yield return new WaitForSeconds(_rightClickHoldLimit);

        OnRightMouseClick?.Invoke(false);

        _rightClickBlocked = true;

        yield return new WaitUntil(() => !Mouse.current.rightButton.isPressed);

        _rightClickBlocked = false;
        _rightClickTimeoutCoroutine = null;
    }

    private void StopTimeout()
    {
        if (_rightClickTimeoutCoroutine != null)
        {
            _coroutineHost.StopCoroutine(_rightClickTimeoutCoroutine);
            _rightClickTimeoutCoroutine = null;
        }
        _rightClickBlocked = false;
    }
}
