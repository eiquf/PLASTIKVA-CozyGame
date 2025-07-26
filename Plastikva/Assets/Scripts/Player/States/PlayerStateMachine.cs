public class PlayerStateMachine
{
    private IPlayerState _current;

    public void SetState(IPlayerState state)
    {
        _current?.Exit();
        _current = state;
        _current.Enter();
    }
    public void Tick() => _current?.Execute();
}
