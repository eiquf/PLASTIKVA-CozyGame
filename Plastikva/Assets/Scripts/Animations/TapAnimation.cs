using DG.Tweening;
using System;
using UnityEngine;

public class TapAnimation : IAnimation<Transform>
{
    private readonly float _scaleFactor = 0.9f;
    private readonly float _animationDuration = 0.1f;

    public void PlayAnimation(Transform tr, Action onComplete)
    {
        if (tr == null) return;

        tr.DOKill(false);

        Vector3 targetScale = tr.localScale * _scaleFactor;

        var seq = DOTween.Sequence();
        seq.Append(tr.DOScale(targetScale, _animationDuration).SetEase(Ease.OutCirc));
        seq.Append(tr.DOScale(tr.localScale, _animationDuration).SetEase(Ease.InOutCirc));

        seq.SetLink(tr.gameObject, LinkBehaviour.KillOnDestroy);

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
