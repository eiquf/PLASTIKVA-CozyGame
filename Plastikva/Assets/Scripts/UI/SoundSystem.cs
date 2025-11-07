using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundSystem : MonoBehaviour
{
    private ISaveService _save;
    private UI _ui;

    [SerializeField] List<AudioSource> _sources = new();
    private bool _isShut = false;

    private readonly SoundModel _model = new();
    private readonly SoundView _view = new();

    [Inject]
    private void Container(ISaveService save, UI ui)
    {
        _save = save;
        _ui = ui;
    }
    public void Initialize()
    {
        _view.SetUp(_ui);
        _model.SetUp(_save);

        bool isMuted = _save.Data.sound == 0;
        _view.Execute(isMuted);

        _ui.SoundButton.onClick.AddListener(Execute);
    }

    private void Execute()
    {
        _isShut = !_isShut;

        _view.Execute(_isShut);

        foreach (var item in _sources)
            item.volume = _model.ChangeVolume(_isShut);  
    }
}
