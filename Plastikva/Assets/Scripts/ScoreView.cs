using R3;
using System;
using UnityEngine.UI;
using Zenject;

public class ScoreView : IDisposable
{
    private readonly ReactiveProperty<int> _score = new(0);
    private readonly CompositeDisposable _disposables = new();

    private Image _bar;
    private int _max = 1;

   public void SetUp(UI ui)
    {
        _bar = ui.ScoreBar;
        _score.Subscribe(_ => UpdateView()).AddTo(_disposables);
        UpdateView();
    }

    public void SetMax(int max) => _max = Math.Max(1, max);
    public void ResetScore() => _score.Value = 0;
    public void ChangeCount(int delta) => _score.Value += delta;

    private void UpdateView()
    {
        if (_bar == null) return;
        float fill = Math.Clamp(_score.Value / (float)_max, 0f, 1f);
        _bar.fillAmount = fill;
    }
    public void Dispose() => _disposables.Dispose();
}
