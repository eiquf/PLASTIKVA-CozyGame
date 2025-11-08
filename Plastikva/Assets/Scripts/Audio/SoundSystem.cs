using R3;
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

    [SerializeField] private SoundLibrary _library;

    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(ISaveService save, UI ui)
    {
        _save = save;
        _ui = ui;
    }

    public void Initialize(List<ISound> sounds)
    {
        _view.SetUp(_ui);
        _model.SetUp(_save);

        if(_save.Data.isFirstLaunch) _save.Data.sound = 1;

        _isShut = _save.Data.sound == 0;

        _view.Execute(_isShut);

        foreach (var source in _sources)
            source.volume = _model.ChangeVolume(_isShut);

        _ui.SoundButton.onClick.AddListener(Execute);

        _library.Initialize();

        foreach (var soundSource in sounds)
        {
            soundSource.PlayCommand
                .Subscribe(index =>
                {
                    if (_isShut) return;
                    PlaySound(index);
                })
                .AddTo(_disposables);
        }
    }

    private void Execute()
    {
        _isShut = !_isShut;              
        _view.Execute(_isShut);
        foreach (var item in _sources)
            item.volume = _model.ChangeVolume(_isShut);
    }

    private void PlaySound(int index)
    {
        var soundType = (GameSound)index;
        var clip = _library.GetClip(soundType);

        if (clip == null) return;
        if (_sources.Count == 0) return;

        _sources[0].PlayOneShot(clip);
    }
    public bool IsMuted => _isShut;

    public void MuteAll(bool mute, bool stopCurrentlyPlaying = true)
    {
        _isShut = mute;
        _save.Data.sound = mute ? 0 : 1;

        _view.Execute(_isShut);

        foreach (var src in _sources)
        {
            src.volume = _model.ChangeVolume(_isShut);
            if (mute && stopCurrentlyPlaying)
                src.Stop();
        }
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        if (_ui != null) _ui.SoundButton.onClick.RemoveListener(Execute);
    }
}
