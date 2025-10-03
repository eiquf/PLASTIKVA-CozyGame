public class SwimMovementState : IPlayerState
{
    private readonly IPlayerContext _context;
    private readonly IMovementBehaviour _movement;

    private bool _isMoving = false;
    public SwimMovementState(IPlayerContext context)
    {
        _context = context;
        _movement = new PlayerMovement(context);
    }
    public void Enter() { } /*=> Animation();*/
    public void Exit() { } /*=> Animation();*/

    public void Execute()
    {
        var input = _context.CurrentInput;

        _isMoving = input.magnitude != 0;

        if (_isMoving) _movement.Execute(input, _context.IsSprinting);
        else _context.StateMachine.SetState(_context.IdleState);
    }
    private void Animation()
    {
        _context.Animator.SetBool(PlayerConfigs.IDLE_ANIM, _isMoving);
    }
}
