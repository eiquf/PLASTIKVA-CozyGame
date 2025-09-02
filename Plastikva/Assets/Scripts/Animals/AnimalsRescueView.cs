using System.Text;
using TMPro;

public class AnimalsRescueView
{
    private TextMeshProUGUI _count;
    private readonly StringBuilder _sb = new(16);
    private UI _ui;
    public void Setup(UI ui)
    {
        _ui = ui;
        _count = _ui.AnimalsCountText;
    }

    #region Animals Count
    public void Render(int score) => UpdateView(score);
    private void UpdateView(int score)
    {
        if (_count == null) return;

        _sb.Clear();
        _sb.Append(score);
        _sb.Append(" /5x");

        _count.text = _sb.ToString();
    }
    #endregion
}
