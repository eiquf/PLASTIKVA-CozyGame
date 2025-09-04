using System.Text;
using TMPro;
using UnityEngine;

public class LevelGeneratorView
{
    private TextMeshProUGUI _text;
    private CanvasGroup _canvasGroup;
    private readonly AnimationContext<CanvasGroup> _animContext = new();
    private readonly StringBuilder _sb = new();

    public void Setup(UI ui)
    {
        _canvasGroup = ui.CanvasGroup;
        _text = ui.LevelText;
        _animContext.SetAnimationStrategy(new ShowupAnimation());
    }
    public void ShowPanel(string name)
    {
        _sb.Clear();
        _sb.Append(name);
        _text.text = _sb.ToString();

        _animContext.PlayAnimation(_canvasGroup);
    }
}
