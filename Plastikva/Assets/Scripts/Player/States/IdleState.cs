public class IdleState : IPlayerState
{
    private readonly IPlayerContext _context;
    private readonly PlayerAnim _anim;
    public IdleState(IPlayerContext context, PlayerAnim anim)
    {
        _context = context;
        _anim = anim;
    }
    public void Enter()
    {
        bool facingRight = _context.SwimState is not SwimMovementState swim || (swim.FacingRight());

        _anim.Set(facingRight: facingRight, moving: false);
    }
    public void Execute()
    {
        var input = _context.CurrentInput;
        bool isIdle = input.sqrMagnitude <= 0.0001f;

        if (!isIdle)
            _context.StateMachine.SetState(_context.SwimState);
        else
        {
            bool facingRight = GetFacingRightFromLastKnown();
            _anim.Set(facingRight, false);
        }
    }

    public void Exit() { }
    private bool GetFacingRightFromLastKnown() => true;
}