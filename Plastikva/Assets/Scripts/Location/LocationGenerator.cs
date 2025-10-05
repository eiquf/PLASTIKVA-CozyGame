using R3;
using UnityEngine;
using Zenject;

public class LocationGenerator : MonoBehaviour
{
    private readonly LevelGeneratorModel _model = new();
    private readonly LevelGeneratorView _view = new();

    private LevelUnlocking _unlocking;
    private UI _ui;

    [SerializeField] private Transform[] _stuffPos;
    [SerializeField] private Transform[] _planes;
    [SerializeField] private Transform[] _walls;
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

        _model.SetupPrefs(_trashPref, _animalPref, _save, _stuffPos);
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

            _model.SetupData(level.Trash, level.Animals);
            _model.SetupPlane(plane);
            _model.Generate();
        })
        .AddTo(_disposables);

        _unlocking.CurrentLevel
          .Skip(1)
            .Subscribe(level =>
            {
                var idx = Mathf.Clamp(level.ID, 0, _walls.Length - 1);
                var wall = _walls[idx];
                wall.gameObject.SetActive(false);
            })
            .AddTo(_disposables);
    }
    private void OnDestroy() => _disposables.Dispose();
}