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

    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking, UI ui)
    {
        _unlocking = unlocking;
        _ui = ui;
    }

    public void Initialize()
    {
        _view.Setup(_ui);

        _unlocking.CurrentLevel
    .Subscribe(level =>
    {
        if (level == null) return;

        _view.ShowPanel(level.Title);

        var idx = Mathf.Clamp(level.ID, 0, _planes.Length - 1);
        if (_planes == null || _planes.Length == 0 || _planes[idx] == null)
        {
            Debug.LogError("Planes not set or invalid level.ID");
            return;
        }

        _model.Setup(level.Trash, _planes[idx]);
        _model.Generate();
    })
    .AddTo(_disposables);
    }
    private void OnDestroy() => _disposables.Dispose();
}