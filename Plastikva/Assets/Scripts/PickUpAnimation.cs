using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PickUpAnimation
{
    private Transform _target;
    private GameObject _starPrefab;
    private Transform _pos;

    [Header("Settings")]
    [SerializeField] private float moveDuration = 0.8f;
    [SerializeField] private float spreadRadius = 1f;

    private ObjectPool<GameObject> _coinPool;

    public void SetUp(UI ui)
    {
        _target = ui.ScoreBar.transform;
        _starPrefab = ui.Star;
        _pos = ui.StarsPos;

        _coinPool ??= new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var go = Object.Instantiate(_starPrefab,_pos);
                    go.SetActive(false);
                    return go;
                },
                actionOnGet: go =>
                {
                    go.SetActive(true);
                    go.transform.SetParent(_pos, false);
                    go.transform.localScale = Vector3.one;
                },
                actionOnRelease: go =>
                {
                    go.transform.DOKill(true);
                    go.SetActive(false);
                },
                actionOnDestroy: go =>
                {
                    Object.Destroy(go);
                },
                collectionCheck: false,
                defaultCapacity: 32,
                maxSize: 256
            );
    }

    public void Prewarm(int count)
    {
        var tmp = new List<GameObject>(count);
        for (int i = 0; i < count; i++) tmp.Add(_coinPool.Get());
        for (int i = 0; i < tmp.Count; i++) _coinPool.Release(tmp[i]);
    }

    public void PlayCollectAnimation(Vector3 worldStartPos, int count)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldStartPos);

        for (int i = 0; i < count; i++)
        {
            GameObject coin = _coinPool.Get();
            var rect = coin.transform as RectTransform;

            Vector3 startPos = screenPos + 100f * spreadRadius * (Vector3)Random.insideUnitCircle;
            rect.position = startPos;

            Vector3 targetPos = (_target as RectTransform).position;

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(Random.Range(0f, 0.2f));
            seq.Append(rect.DOMove(targetPos, moveDuration).SetEase(Ease.InOutQuad));
            seq.Join(rect.DOScale(0.5f, moveDuration).From(1f).SetEase(Ease.InBack));

            seq.OnComplete(() =>
            {
                _coinPool.Release(coin);
            });
        }
    }
}
