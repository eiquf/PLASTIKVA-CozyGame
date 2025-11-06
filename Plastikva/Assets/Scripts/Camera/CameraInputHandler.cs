using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Zenject;

public class CameraInputHandler : IDisposable, IInitializable
{
    private readonly InputController _controller;
    private readonly float _rightClickHoldLimit = 0.2f;
    private Coroutine _rightClickTimeoutCoroutine;
    private bool _rightClickBlocked;

    private readonly MonoBehaviour _coroutineHost;

    private readonly bool _isMobile;

    public event Action<bool> OnLeftMouseClick;
    public event Action<bool> OnRightMouseClick;

    [Inject]
    public CameraInputHandler(InputController inputController, [Inject(Id = "CoroutineHost")] MonoBehaviour coroutineHost)
    {
        _controller = inputController;
        _coroutineHost = coroutineHost;
        _isMobile = Application.isMobilePlatform;
    }

    public Vector2 Delta()
    {
        if (_isMobile)
        {
            if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0)
                return Vector2.zero;

            return Touchscreen.current.touches[0].delta.ReadValue();
        }

        return _controller.Input.Camera.MouseDelta.ReadValue<Vector2>();
    }

    public Vector2 DeltaScroll() => _controller.Input.Camera.MouseScroll.ReadValue<Vector2>();

    public void Initialize()
    {
        if (!_isMobile)
        {
            _controller.Input.Camera.MouseClick.performed += OnMouseClick;
            _controller.Input.Camera.MouseClick.canceled += OnMouseClick;
        }
        else
            InputSystem.onEvent += OnTouchEvent;
    }

    public void Dispose()
    {
        if (!_isMobile)
        {
            _controller.Input.Camera.MouseClick.performed -= OnMouseClick;
            _controller.Input.Camera.MouseClick.canceled -= OnMouseClick;
        }
        else
            InputSystem.onEvent -= OnTouchEvent;
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

    private void OnTouchEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is not Touchscreen touchDevice) return;

        foreach (var touchControl in touchDevice.touches)
        {
            var phase = touchControl.phase.ReadValue();
            var index = touchControl.touchId.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (index == 0)
                    OnLeftMouseClick?.Invoke(true);
                else if (index == 1)
                    OnRightMouseClick?.Invoke(true);
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                if (index == 0)
                    OnLeftMouseClick?.Invoke(false);
                else if (index == 1)
                    OnRightMouseClick?.Invoke(false);
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
