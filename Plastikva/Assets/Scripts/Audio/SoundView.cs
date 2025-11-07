using UnityEngine;
using UnityEngine.UI;

public class SoundView
{
    Sprite[] _sprites = new Sprite[2];
    Button _button;

    public void SetUp(UI ui)
    {
        _sprites = ui.SoundSprites;
        _button = ui.SoundButton;
    }
    public void Execute(bool state)
    {
        if (_sprites.Length >= 2)
        {
            _button.image.sprite = state
                ? _sprites[1]
                : _sprites[0];
        }
    }
}