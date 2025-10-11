using UnityEngine;

public sealed class PlayerAnim
{
    private readonly Animator _anim;

    private bool _lastFacingRight;
    private bool _lastMoving;
    private bool _dirty = true;

    private static readonly int HASH_SWIM_FRONT = Animator.StringToHash(PlayerConfigs.SWIM_FRONT);
    private static readonly int HASH_SWIM_BACK = Animator.StringToHash(PlayerConfigs.SWIM_BACK);

    public PlayerAnim(Animator animator) => _anim = animator;

    public void Set(bool facingRight, bool moving)
    {
        if (!_dirty && facingRight == _lastFacingRight && moving == _lastMoving)
            return;

        _lastFacingRight = facingRight;
        _lastMoving = moving;
        _dirty = false;

        _anim.SetBool(HASH_SWIM_FRONT, false);
        _anim.SetBool(HASH_SWIM_BACK, false);

        if (moving)
        {
            if (facingRight) _anim.SetBool(HASH_SWIM_BACK, true);  
            else _anim.SetBool(HASH_SWIM_FRONT, true);   
        }
        else
        {
            if (facingRight) _anim.SetBool(HASH_SWIM_BACK, false);  
            else _anim.SetBool(HASH_SWIM_FRONT, false); 
        }
    }

    public void MarkDirty() => _dirty = true;
}
