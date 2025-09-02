using System;
using UnityEngine.UI;

public class ScoreView
{
    private Image _bar;
    private readonly int _max = 100;

    public void SetUp(UI ui)
    {
        _bar = ui.ScoreBar;
        UpdateView(0);
    }

    public void Render(int score) => UpdateView(score);

    private void UpdateView(int score)
    {
        if (_bar == null) return;
        float fill = Math.Clamp(score / (float)_max, 0f, 1f);
        _bar.fillAmount = fill;
    }
}
