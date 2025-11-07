using DG.Tweening;
using System;
using UnityEngine;

public class ButtonsPopUpAnimation : IAnimation<Transform>
{
    private bool _isOpening;
    private const float POP_HEIGHT = 80f;     
    private const float POP_TIME = 0.4f;      
    private const float DELAY_STEP = 0.08f;   
    private const float SCALE_POP = 1.1f;     

    //also red flag
    public void SetBool(bool isOpening) => _isOpening = isOpening;

    public void PlayAnimation(Transform buttonParent, Action onComplete = null)
    {
        if (buttonParent == null) return;

        int childCount = buttonParent.childCount;
        if (childCount == 0) return;

        int start = _isOpening ? 0 : childCount - 1;
        int end = _isOpening ? childCount : -1;
        int step = _isOpening ? 1 : -1;

        for (int i = start, order = 0; i != end; i += step, order++)
        {
            var child = buttonParent.GetChild(i) as RectTransform;
            if (child == null) continue;

            child.DOKill(true);

            Vector2 originalPos = child.anchoredPosition;
            Vector2 hiddenPos = new(originalPos.x, originalPos.y - POP_HEIGHT);

            if (_isOpening)
            {
                child.anchoredPosition = hiddenPos;
                child.localScale = Vector3.zero;
                child.gameObject.SetActive(true);

                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(order * DELAY_STEP);
                seq.Append(child.DOAnchorPos(originalPos, POP_TIME).SetEase(Ease.OutBack));
                seq.Join(child.DOScale(SCALE_POP, POP_TIME * 0.5f).SetEase(Ease.OutBack));
                seq.Append(child.DOScale(1f, 0.2f));
                seq.OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(order * DELAY_STEP);
                seq.Append(child.DOAnchorPos(hiddenPos, POP_TIME * 0.6f).SetEase(Ease.InBack));
                seq.Join(child.DOScale(0f, 0.3f).SetEase(Ease.InBack));
                seq.OnComplete(() =>
                {
                    child.anchoredPosition = originalPos;
                    child.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
            }
        }
    }
}
