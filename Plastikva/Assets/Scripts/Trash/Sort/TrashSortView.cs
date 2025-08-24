using R3;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrashSortView : IDisposable
{
    private readonly Subject<Unit> _yesClicks = new();
    private readonly Subject<Unit> _noClicks = new();
    public Observable<Unit> YesClicks => _yesClicks;
    public Observable<Unit> NoClicks => _noClicks;

    private readonly StringBuilder _sb = new();

    private Image _image;
    private Button _yesButton;
    private Button _noButton;
    private TextMesh _finalText;

    private UnityAction _yesHandler;
    private UnityAction _noHandler;

    public void SetUp(UI ui)
    {
        _image = ui.TrashImage;
        _yesButton = ui.YesButton;
        _noButton = ui.NoButton;
        _finalText = ui.FinalText;

        _yesHandler = () => _yesClicks.OnNext(Unit.Default);
        _noHandler = () => _noClicks.OnNext(Unit.Default);
        _yesButton.onClick.AddListener(_yesHandler);
        _noButton.onClick.AddListener(_noHandler);

        _finalText.text = string.Empty;
        SetButtonsInteractable(true);
    }

    public void Dispose()
    {
        if (_yesButton != null && _yesHandler != null)
            _yesButton.onClick.RemoveListener(_yesHandler);
        if (_noButton != null && _noHandler != null)
            _noButton.onClick.RemoveListener(_noHandler);

        _yesClicks.OnCompleted();
        _noClicks.OnCompleted();
    }

    public void SetSprite(Sprite sprite)
    {
        if (_image != null) _image.sprite = sprite;
        if (_finalText != null && sprite != null)
            _finalText.text = string.Empty;
    }

    public void ShowFinal(int score, int total)
    {
        if (_finalText == null) return;

        _sb.Clear();
        _sb.Append("Wooooah! You've got: ")
           .Append(score)
           .Append(" from ")
           .Append(total)
           .Append(" max");
        _finalText.text = _sb.ToString();
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (_yesButton != null) _yesButton.interactable = interactable;
        if (_noButton != null) _noButton.interactable = interactable;
    }
}
