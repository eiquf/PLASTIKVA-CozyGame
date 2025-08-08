using System;
using Zenject;

public class InputController : IInitializable, IDisposable
{
    protected GameInput InputActions { get; private set; }
    public GameInput Input => InputActions;
    private InputController() { }
    public void Dispose()
    {
        InputActions?.Disable();
        InputActions?.Dispose();
        InputActions = null;
    }

    public void Initialize()
    {
        InputActions = new GameInput();
        InputActions.Enable();
    }
}
