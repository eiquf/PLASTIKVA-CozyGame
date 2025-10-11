public class SwimMovementState : IPlayerState
{
    private readonly IPlayerContext _context;
    private readonly IMovementBehavior _movement;
    private readonly PlayerAnim _anim;

    private bool _isMoving = false;
    public SwimMovementState(IPlayerContext context, PlayerAnim anim)
    {
        _context = context;
        _movement = new PlayerMovement(context);
        _anim = anim;
    }
    public void Enter() { }
    public void Exit() { }

    public void Execute()
    {
        var input = _context.CurrentInput;

        _isMoving = input.magnitude != 0;

        if (_isMoving)
        {
            _movement.Execute(input, _context.IsSprinting);
            _anim.Set(!_movement.FacingRight(), true);
        }
        else _context.StateMachine.SetState(_context.IdleState);
    }
    public bool FacingRight() => _movement.FacingRight();

}
