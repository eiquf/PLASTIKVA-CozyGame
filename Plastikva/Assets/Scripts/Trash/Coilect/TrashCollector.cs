using R3;
using UnityEngine;
using Zenject;

public class TrashCollector : MonoBehaviour, IScore
{
    private TrashInputHandler _input;

    private readonly TrashView _view = new();
    private readonly TrashCollectorModel _model = new();

    private LevelUnlocking _levelUnlocking;

    private TrashLevelDef _currentLevel;

    private readonly LayerMask TrashMask = 1 << 6;
    private IHitDetector _hitDetector;

    private readonly CompositeDisposable _disposables = new();
    private ReactiveProperty<int> _count = new();

    public ReactiveCommand TakenCommand { get; } = new ReactiveCommand();

    [Inject]
    private void Container(LevelUnlocking levelUnlocking, TrashInputHandler input)
    {
        _input = input;
        _levelUnlocking = levelUnlocking;
    }
    public void Initialize()
    {
        _input.LeftMouseClicked += Collect;

        _hitDetector = new CameraRayHitDetector(Camera.main);
        _count = new ReactiveProperty<int>(1);

        _levelUnlocking.CurrentLevel
              .Subscribe(level =>
              {
                  _currentLevel = level;
                  _count.Value = _currentLevel.RequiredTrashCount;
                  _model.UpdateGoal(_count.Value);
                  _view.SetMaxCount(_count.Value);
              })
              .AddTo(_disposables);

        _model.CurrentCount
            .Subscribe(count =>
            {
                _view.SetCurrentCount(count);
                TakenCommand.Execute(Unit.Default);
            })
            .AddTo(_disposables);

        _model.AllCollected
            .Where(collected => collected)
            .Subscribe(_ => _levelUnlocking.ReportTrashCollected())
            .AddTo(_disposables);

    }
    private void Collect()
    {
        Vector3 mousePos = _input.GetMousePosition();

        bool collected = _hitDetector.TryHit(TrashMask, mousePos);

        if (collected == true)
            _model.Collect();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _input.LeftMouseClicked -= Collect;
    }
}
