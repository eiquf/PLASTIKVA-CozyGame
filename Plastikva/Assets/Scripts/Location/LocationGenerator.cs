using R3;
using UnityEngine;
using Zenject;

public class LocationGenerator : MonoBehaviour
{
    private readonly LevelGeneratorModel _model = new();
    private readonly LevelGeneratorView _view = new();

    private LevelUnlocking _unlocking;
    private UI _ui;

    [SerializeField] private Transform[] _planes;
    [SerializeField] private GameObject _trashPref;
    [SerializeField] private GameObject _animalPref;

    private ISaveService _save;
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking, UI ui)
    {
        _unlocking = unlocking;
        _ui = ui;
    }

    public void Initialize(ISaveService save)
    {
        _save = save;

        _model.Setup(_trashPref, _animalPref, _save);
        _view.Setup(_ui);

        _unlocking.CurrentLevel
        .Subscribe(level =>
        {
            if (level == null) return;
        
            _view.ShowPanel(level.Title);
        
            if (_planes == null || _planes.Length == 0)
                return;
        
            var idx = Mathf.Clamp(level.ID, 0, _planes.Length - 1);
            var plane = _planes[idx];
            if (plane == null)
                return;
        
            _model.SetupData(level.Trash, level.Animals, plane);
            _model.Generate();
        })
        .AddTo(_disposables);

    }
    private void OnDestroy() => _disposables.Dispose();
}