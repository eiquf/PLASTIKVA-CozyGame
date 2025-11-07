using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrashCollector : MonoBehaviour, IScore, ISound
{
#if UNITY_EDITOR
    public bool finished;
#endif
    private TrashInputHandler _input;
    private readonly TrashView _view = new();
    private readonly TrashCollectorModel _model = new();

    private readonly AnimationContext<Transform> _animationContext = new();

    private LevelUnlocking _levelUnlocking;
    private TrashLevelDef _currentLevel;

    private UI _ui;
    private readonly LayerMask TrashMask = 1 << 6;
    private IHitDetector _hitDetector;

    private ISaveService _save;

    private readonly CompositeDisposable _disposables = new();
    public ReactiveCommand<int> TakenCommand { get; } = new ReactiveCommand<int>();
    public ReactiveCommand<int> PlayCommand { get; } = new ReactiveCommand<int>();

    private readonly PickUpAnimation _pickUpAnimation = new();

    [Inject]
    private void Container(LevelUnlocking levelUnlocking, TrashInputHandler input, UI ui)
    {
        _input = input;
        _levelUnlocking = levelUnlocking;
        _ui = ui;
    }

    public void Initialize(ISaveService save)
    {
        _save = save;
        _input.LeftMouseClicked += Collect;

        _pickUpAnimation.SetUp(_ui);
        _pickUpAnimation.Prewarm(ScoresConst.DEFAULT);

        _hitDetector = new CameraRayHitDetector(Camera.main);
        _animationContext.SetAnimationStrategy(new TapAnimation());

        _view.SetUp(_ui);
        _model.Setup(_save);

#if UNITY_EDITOR
        if (finished == true)
        {
            _levelUnlocking.ReportTrashCollected();
        }
#endif

        _model.CurrentCount
              .DistinctUntilChanged()
              .Subscribe(count =>
              {
                  _view.Render(count);
                  TakenCommand.Execute(ScoresConst.DEFAULT);
              })
              .AddTo(_disposables);

        _model.AllCollected
              .Where(collected => collected)
              .Subscribe(_ => _levelUnlocking.ReportTrashCollected())
              .AddTo(_disposables);

        _levelUnlocking.CurrentLevel
            .Take(1)
            .Subscribe(level =>
            {
                _currentLevel = level;
                if (_currentLevel == null) return;

                _model.UpdateGoal(_currentLevel.RequiredTrashCount, resetProgress: false);
            })
            .AddTo(_disposables);

        _levelUnlocking.CurrentLevel
          .Skip(1)
            .Subscribe(level =>
            {
                _currentLevel = level;
                if (_currentLevel == null) return;

                _model.UpdateGoal(_currentLevel.RequiredTrashCount, resetProgress: true);
                _view.Render(0);
            })
            .AddTo(_disposables);
    }

    private void Collect()
    {
        if (_save == null || _save.Data == null) return;
        _save.Data.collectedTrashIds ??= new List<int>();

        if (Camera.main == null) return;

        Vector3 mousePos = _input.GetMousePosition();

        var hitGo = _hitDetector.TryGetHitObject(TrashMask, mousePos);
        if (hitGo == null) return;

        var inst = hitGo.GetComponentInParent<TrashInstance>();
        if (inst == null) return;

        var rootGo = inst.gameObject;

        var list = _save.Data.collectedTrashIds;
        if (!list.Contains(inst.Id))
        {
            _animationContext.PlayAnimation(rootGo.transform, () =>
            {
                _model.Collect();
                Destroy(rootGo);
            });

            PlayCommand.Execute((int)GameSound.Trash_PickUp);

            _pickUpAnimation.PlayCollectAnimation(rootGo.transform.position, ScoresConst.DEFAULT);
            _pickUpAnimation.PlayCenterToDeliverAnimation(inst.sprite.sprite, onDelivered: () => { Debug.Log("trash"); });

            list.Add(inst.Id);
            _save.Save();
        }
    }


    private void OnDestroy()
    {
        _disposables.Dispose();
        _input.LeftMouseClicked -= Collect;
    }
}