using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class AnimalsInputHandler : IInitializable, IDisposable
{
    private readonly InputController _controller;

    public event Action Rescued;
    public event Action Help;

    private InputAction _helpAction;
    private InputAction _rescueAction;   
    private InputAction _pointerPosAction;

    [Inject]
    public AnimalsInputHandler(InputController controller) => _controller = controller;

    public void Initialize()
    {
        var gameplay = _controller.Input.Gameplay;
        _helpAction = gameplay.Collecting; 
        _rescueAction = gameplay.Rescue;     
        _pointerPosAction = gameplay.PointerPos;

        _helpAction.performed += _ => Help?.Invoke();
        _rescueAction.performed += _ => Rescued?.Invoke();

        _helpAction.Enable();
        _rescueAction.Enable();
        _pointerPosAction.Enable(); 
    }

    public Vector2 GetMousePosition() => _pointerPosAction?.ReadValue<Vector2>() ?? Vector2.zero;

    public void Dispose()
    {
        _helpAction.performed -= _ => Help?.Invoke();
        _rescueAction.performed -= _ => Rescued?.Invoke();

        _helpAction.Disable();
        _rescueAction.Disable();
        _pointerPosAction.Disable();
    }
}
