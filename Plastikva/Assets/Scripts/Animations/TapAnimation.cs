using DG.Tweening;
using System;
using UnityEngine;

public class TapAnimation : IAnimation<Transform>
{
    private readonly float _scaleFactor = 0.9f;
    private readonly float _animationDuration = 0.1f;

    private Vector3 _originalScale = Vector3.zero;

    public void PlayAnimation(Transform tr, Action onComplete)
    {
        if (tr == null) return;

        if (_originalScale == Vector3.zero)
            _originalScale = tr.localScale;

        tr.DOKill(false);

        Vector3 targetScale = _originalScale * _scaleFactor;

        var seq = DOTween.Sequence();
        seq.Append(tr.DOScale(targetScale, _animationDuration).SetEase(Ease.OutCirc));
        seq.Append(tr.DOScale(_originalScale, _animationDuration).SetEase(Ease.InOutCirc));

        seq.SetLink(tr.gameObject, LinkBehaviour.KillOnDestroy);
        seq.OnComplete(() => onComplete?.Invoke());
    }
}
