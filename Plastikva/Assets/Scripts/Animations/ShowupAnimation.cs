using DG.Tweening;
using UnityEngine;

public class ShowupAnimation : IAnimation<CanvasGroup>
{
    private readonly float _fadeDuration = 0.5f;
    private readonly float _pauseDuration = 2f;
    public void PlayAnimation(CanvasGroup canvasGroup)
    {
        canvasGroup.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, _fadeDuration))    
           .AppendInterval(_pauseDuration)                 
           .Append(canvasGroup.DOFade(0f, _fadeDuration));
    }
}