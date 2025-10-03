public class IdleState : IPlayerState
{
    private readonly IPlayerContext _context;

    private bool _isIdle = false;
    public IdleState(IPlayerContext context) => _context = context;
    public void Enter() { } /*=> Animation();*/
    public void Execute()
    {
        var input = _context.CurrentInput;
        float vectorLenght = input.magnitude;

        _isIdle = vectorLenght == 0;

        if (!_isIdle) _context.StateMachine.SetState(_context.SwimState);
    }

    public void Exit() { }/* => Animation();*/
    //private void Animation() => _context.Animator.SetBool(PlayerConfigs.IDLE_ANIM, _isIdle);
}