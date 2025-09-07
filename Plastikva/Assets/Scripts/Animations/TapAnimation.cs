using DG.Tweening;
using UnityEngine;

public class TapAnimation : IAnimation<Transform>
{
    private readonly float _scaleFactor = 0.9f; 
    private readonly float _animationDuration = 0.1f;

    public void PlayAnimation(Transform tr)
    {
        if (tr == null) return;

        tr.DOKill(true);

        var baseScale = tr.localScale;
        var targetScale = baseScale * _scaleFactor;

        tr.DOScale(targetScale, _animationDuration)
          .SetEase(Ease.OutCirc)
          .OnComplete(() =>
          {
              tr.DOScale(baseScale, _animationDuration).SetEase(Ease.InOutCirc);
          });
    }
}
