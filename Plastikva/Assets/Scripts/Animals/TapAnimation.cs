using DG.Tweening;
using UnityEngine;
public class TapAnimation : IAnimation
{
    private readonly float _scaleFactor = 0.9f;
    private readonly float _animationDuration = 0.1f;
    public void PlayAnimation(Transform tr)
    {
        if (tr == null) return;
        tr.DOKill(false);
        var originalScale = tr.localScale;
        var targetScale = originalScale * _scaleFactor;
        DOTween.Sequence()
            .SetLink(tr.gameObject, LinkBehaviour.KillOnDestroy)
            .Append(tr.DOScale(targetScale, _animationDuration)
            .SetEase(Ease.OutCirc))
            .Append(tr.DOScale(originalScale, _animationDuration)
            .SetEase(Ease.InOutCirc));
    }
}