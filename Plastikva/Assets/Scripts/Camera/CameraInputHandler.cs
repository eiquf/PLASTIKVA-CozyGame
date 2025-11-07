using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraInputHandler : IDisposable, IInitializable
{
    private readonly InputController _controller;
    private readonly float _rightClickHoldLimit = 0.2f;
    private readonly MonoBehaviour _coroutineHost;

    private float _prevMagnitude;
    private Vector2 _prevCenter;
    private Coroutine _rightClickTimeoutCoroutine;
    private bool _rightClickBlocked;

    private readonly bool _isMobile;
    private float _zoomSpeed = 0.02f;

    public event Action<bool> OnLeftMouseClick;
    public event Action<bool> OnRightMouseClick;
    public event Action<float> OnZoom;
    public event Action<Vector2> OnRotate; 

    [Inject]
    public CameraInputHandler(InputController inputController, [Inject(Id = "CoroutineHost")] MonoBehaviour coroutineHost)
    {
        _controller = inputController;
        _coroutineHost = coroutineHost;
        _isMobile = Application.isMobilePlatform;
    }

    public Vector2 Delta() => _controller.Input.Camera.MouseDelta.ReadValue<Vector2>();
    public Vector2 DeltaScroll() => _controller.Input.Camera.MouseScroll.ReadValue<Vector2>();

    public void Initialize()
    {
        if (!_isMobile)
        {
            _controller.Input.Camera.MouseClick.performed += OnMouseClick;
            _controller.Input.Camera.MouseClick.canceled += OnMouseClick;
        }
        else
        {
            _controller.Input.Camera.Touch0Pos.performed += _ => DetectTouch();
            _controller.Input.Camera.Touch1Pos.performed += _ => DetectTouch();
        }
    }

    public void Dispose()
    {
        if (!_isMobile)
        {
            _controller.Input.Camera.MouseClick.performed -= OnMouseClick;
            _controller.Input.Camera.MouseClick.canceled -= OnMouseClick;
        }
        else
        {
            _controller.Input.Camera.Touch0Pos.performed -= _ => DetectTouch();
            _controller.Input.Camera.Touch1Pos.performed -= _ => DetectTouch();
        }
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        bool isDown = context.performed;
        var control = context.control;

        if (control == Mouse.current.leftButton)
        {
            OnLeftMouseClick?.Invoke(isDown);
        }
        else if (control == Mouse.current.rightButton)
        {
            if (_rightClickBlocked) return;

            if (isDown)
            {
                OnRightMouseClick?.Invoke(true);
                StopTimeout();
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

    private void DetectTouch()
    {
        var screen = Touchscreen.current;
        if (screen == null) return;

        var touches = screen.touches;
        int touchCount = touches.Count;

        if (touchCount == 0)
        {
            _prevMagnitude = 0f;
            _prevCenter = Vector2.zero;
            return;
        }

        if (touchCount == 1)
        {
            var touch = touches.ElementAt(0);
            var phase = touch.phase.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
                OnLeftMouseClick?.Invoke(true);
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                OnLeftMouseClick?.Invoke(false);
        }

        else if (touchCount >= 2)
        {
            var touch0 = touches.ElementAt(0).position.ReadValue();
            var touch1 = touches.ElementAt(1).position.ReadValue();

            var magnitude = (touch0 - touch1).magnitude;
            if (_prevMagnitude == 0f)
                _prevMagnitude = magnitude;
            var difference = magnitude - _prevMagnitude;
            OnZoom?.Invoke(-difference * _zoomSpeed);
            _prevMagnitude = magnitude;

            var center = (touch0 + touch1) * 0.5f;
            if (_prevCenter == Vector2.zero)
                _prevCenter = center;

            var delta = center - _prevCenter;
            if (delta.sqrMagnitude > 0.01f) 
                OnRotate?.Invoke(delta);

            _prevCenter = center;
        }
    }
}
