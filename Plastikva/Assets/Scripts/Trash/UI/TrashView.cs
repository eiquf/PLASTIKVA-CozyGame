using TMPro;
using System.Text;

public class TrashView
{
    private TextMeshProUGUI _count;
    private readonly StringBuilder _sb = new(16);

    public void SetUp(UI ui) => _count = ui.TrashCountText;

    public void Render(int score) => UpdateView(score);

    private void UpdateView(int score)
    {
        if (_count == null) return;

        _sb.Clear();
        _sb.Append(score);
        _sb.Append(" /7x");

        _count.text = _sb.ToString();
    }
}