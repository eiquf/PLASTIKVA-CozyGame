using UnityEngine;

public class EnemyMovement
{
    float switchTimer = 0f;
    private float timeFar = 0f;

    private bool movingToPlayer = true;
    private float offsetTime;
    private Vector3 _startPos;

    private IEnemyContext _context;
    public void Initialize(Vector3 startPos, IEnemyContext context)
    {
        _startPos = startPos;
        _context = context;

        offsetTime = Random.Range(0f, _context.DesyncDelay);
        movingToPlayer = context.MoveToPlayer;
    }
    public void Execute()
    {
        var followTarget = _context.FollowTarget;
        if (followTarget == null || _context.Pos == null) return;

        float swimY = Mathf.Sin((Time.time + offsetTime) * _context.ZigzagFrequency) * _context.ZigzagAmplitude;

        Vector3 target = movingToPlayer
            ? new Vector3(followTarget.position.x, _context.Pos.position.y + swimY, followTarget.position.z)
            : new Vector3(_startPos.x, _context.Pos.position.y + swimY, _startPos.z);

        _context.Pos.position = Vector3.MoveTowards(_context.Pos.position, target, _context.Speed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(_context.Pos.position, followTarget.position);
        float distanceToStart = Vector3.Distance(_context.Pos.position, _startPos);

        if (movingToPlayer && distanceToPlayer <= _context.SwitchDistance)
        {
            switchTimer += Time.deltaTime;
            if (switchTimer >= _context.SwitchDelay)
            {
                movingToPlayer = false;
                switchTimer = 0f;
                timeFar = 0f;
            }
        }
        else if (!movingToPlayer && distanceToStart <= _context.SwitchDistance)
        {
            switchTimer += Time.deltaTime;
            if (switchTimer >= _context.SwitchDelay)
            {
                movingToPlayer = true;
                switchTimer = 0f;
                timeFar = 0f;
            }
        }
        else
            switchTimer = 0f;

        if (!movingToPlayer)
        {
            if (distanceToPlayer > _context.FarDistance)
            {
                timeFar += Time.deltaTime;
                if (timeFar >= _context.ReturnDelay)
                {
                    movingToPlayer = true;
                    timeFar = 0f;
                }
            }
            else
                timeFar = 0f;
        }
        else
            timeFar = 0f;
    }
    public void UpdateMoving() => movingToPlayer = _context.MoveToPlayer;
}