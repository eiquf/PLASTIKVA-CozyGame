using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PickUpAnimation
{
    private Transform _target;              
    private Transform _targetDown;              
    private GameObject _starPrefab;        
    private Transform _pos;                 
    private Transform _centerAnchor;
    private Sprite _defaultStarSprite;

    [Header("Scatter Settings")]
    [SerializeField] private float moveDuration = 0.8f;
    [SerializeField] private float spreadRadius = 1f;

    [Header("Center-to-Deliver Settings")]
    [SerializeField] private float centerPopDuration = 0.25f;
    [SerializeField] private float centerPopOvershoot = 1.15f;
    [SerializeField] private float centerHoldDelay = 0.5f;
    [SerializeField] private float centerFlyDuration = 0.8f;
    [SerializeField] private Ease centerFlyEase = Ease.InQuad;
    [SerializeField] private bool timeScaleIndependent = false;

    private ObjectPool<GameObject> _coinPool;

    public void SetUp(UI ui)
    {
        _target = ui.ScoreBar.transform;
        _targetDown = ui.TrashCountText.transform;
        _starPrefab = ui.Star;
        _pos = ui.StarsPos;
        _centerAnchor = ui.CenterAnchor;

        var prefabImg = _starPrefab != null ? _starPrefab.GetComponent<Image>() : null;
        _defaultStarSprite = prefabImg != null ? prefabImg.sprite : null;

        _coinPool ??= new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var go = UnityEngine.Object.Instantiate(_starPrefab, _pos);
                go.SetActive(false);
                return go;
            },
            actionOnGet: go =>
            {
                go.SetActive(true);
                go.transform.SetParent(_pos, false);
                go.transform.localScale = Vector3.one;
                go.transform.DOKill(true);

                var img = go.GetComponent<Image>();
                if (img != null && _defaultStarSprite != null)
                    img.sprite = _defaultStarSprite;
            },
            actionOnRelease: go =>
            {
                go.transform.DOKill(true);
                var img = go.GetComponent<Image>();
                if (img != null && _defaultStarSprite != null)
                    img.sprite = _defaultStarSprite;

                go.SetActive(false);
            },
            actionOnDestroy: go => UnityEngine.Object.Destroy(go),
            collectionCheck: false,
            defaultCapacity: 32,
            maxSize: 256
        );
    }

    public void Prewarm(int count)
    {
        if (_coinPool == null) return;
        var tmp = new List<GameObject>(count);
        for (int i = 0; i < count; i++) tmp.Add(_coinPool.Get());
        for (int i = 0; i < tmp.Count; i++) _coinPool.Release(tmp[i]);
    }

    public void PlayCollectAnimation(Vector3 worldStartPos, int count)
    {
        if (_coinPool == null || _target == null) return;
        if (Camera.main == null) return;

        var targetRect = _target as RectTransform;
        if (targetRect == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldStartPos);

        for (int i = 0; i < count; i++)
        {
            GameObject coin = _coinPool.Get();
            var rect = coin.transform as RectTransform;
            if (rect == null) { _coinPool.Release(coin); continue; }

            Vector3 startPos = screenPos + 100f * spreadRadius * (Vector3)UnityEngine.Random.insideUnitCircle;
            rect.position = startPos;

            Vector3 targetPos = targetRect.position;

            Sequence seq = DOTween.Sequence().SetUpdate(timeScaleIndependent);
            seq.AppendInterval(UnityEngine.Random.Range(0f, 0.2f));
            seq.Append(rect.DOMove(targetPos, moveDuration).SetEase(Ease.InOutQuad));
            seq.Join(rect.DOScale(0.5f, moveDuration).From(1f).SetEase(Ease.InBack));
            seq.OnComplete(() => _coinPool.Release(coin));
        }
    }

    public void PlayCenterToDeliverAnimation(Sprite overrideSprite, Action onDelivered = null)
    {
        if (_centerAnchor == null) return;
        if (_coinPool == null) return;

        var icon = _coinPool.Get();
        var rect = icon.transform as RectTransform;
        if (rect == null) { _coinPool.Release(icon); return; }

        var img = icon.GetComponent<Image>();
        if (img != null && overrideSprite != null) img.sprite = overrideSprite;

        icon.SetActive(true);
        rect.SetParent(_centerAnchor, false);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.zero;
        rect.DOKill(true);

        if (_targetDown == null) { _coinPool.Release(icon); return; }

        Vector3 targetPos = (_targetDown as RectTransform) != null
            ? ((RectTransform)_targetDown).position
            : _targetDown.position;

        Sequence seq = DOTween.Sequence().SetUpdate(timeScaleIndependent);
        seq.Append(rect.DOScale(centerPopOvershoot, centerPopDuration).SetEase(Ease.OutBack));
        seq.AppendInterval(centerHoldDelay);
        seq.Append(rect.DOMove(targetPos, centerFlyDuration).SetEase(centerFlyEase));
        seq.Join(rect.DOScale(0.7f, centerFlyDuration).SetEase(centerFlyEase));
        seq.OnComplete(() =>
        {
            rect.DOKill(true);
            _coinPool.Release(icon);
            onDelivered?.Invoke();
        });
    }
}
