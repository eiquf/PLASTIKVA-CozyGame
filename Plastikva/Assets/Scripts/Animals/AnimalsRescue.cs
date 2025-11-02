using R3;
using UnityEngine;
using Zenject;

public class AnimalsRescue : MonoBehaviour, IScore
{
#if UNITY_EDITOR
    public bool finished;
#endif
    private AnimalsInputHandler _input;
    private readonly AnimationContext<Transform> _animationContext = new();

    private readonly AnimalsRescueModel _model = new();
    private readonly AnimalsRescueView _view = new();

    private LevelUnlocking _levelUnlocking;
    private TrashLevelDef _currentLevel;

    private AnimalsData[] _animalsData = System.Array.Empty<AnimalsData>();

    private UI _ui;
    private readonly LayerMask AnimalMask = 1 << 8;
    private IHitDetector _hitDetector;

    private ISaveService _save;

    private GameObject _lastAnimal;

    private readonly CompositeDisposable _disposables = new();

    public ReactiveCommand<int> TakenCommand { get; } = new ReactiveCommand<int>();
    private readonly PickUpAnimation _pickUpAnimation = new();

    [Inject]
    private void Container(LevelUnlocking levelUnlocking, AnimalsInputHandler input, UI ui, ISaveService save)
    {
        _input = input;
        _levelUnlocking = levelUnlocking;
        _ui = ui;
        _save = save;
    }

    public void Initialize(ISaveService save)
    {
        _save = save;

        _input.Rescued += Rescue;
        _input.Help += Help;

        _hitDetector = new CameraRayHitDetector(Camera.main);
        _animationContext.SetAnimationStrategy(new TapAnimation());

        _pickUpAnimation.SetUp(_ui);
        _pickUpAnimation.Prewarm(ScoresConst.DEFAULT);

        _view.Setup(_ui);
        _model.Setup(_save);

#if UNITY_EDITOR
        if (finished == true)
        {
            _levelUnlocking.ReportAnimalsRescued();
        }
#endif
        _model.CurrentCount
            .Subscribe(count =>
            {
                _view.Render(count);
                TakenCommand.Execute(ScoresConst.DEFAULT);
            })
            .AddTo(_disposables);

        _model.AllCollected
             .Where(collected => collected)
             .Subscribe(_ => _levelUnlocking.ReportAnimalsRescued())
             .AddTo(_disposables);

        _levelUnlocking.CurrentLevel
            .Take(1)
            .Subscribe(level =>
            {
                _currentLevel = level;
                if (_currentLevel == null) return;

                _model.UpdateGoal(_currentLevel.RequiredAnimalsCount, resetProgress: false);
                _animalsData = level.Animals ?? System.Array.Empty<AnimalsData>();
            })
             .AddTo(_disposables);

        _levelUnlocking.CurrentLevel
          .Skip(1)
          .Subscribe(level =>
          {
              _currentLevel = level;
              if (_currentLevel == null) return;

              _model.UpdateGoal(_currentLevel.RequiredAnimalsCount, resetProgress: true);
              _view.Render(0);

              _animalsData = level.Animals ?? System.Array.Empty<AnimalsData>();
          })
          .AddTo(_disposables);

    }
    private void Help()
    {
        Vector3 mousePos = _input.GetMousePosition();

        bool hit = _hitDetector.TryHit(AnimalMask, mousePos);

        if (!hit)
        {
            _lastAnimal = null;
            return;
        }

        _lastAnimal = _hitDetector.TryGetHitObject(AnimalMask, mousePos);
        if (_lastAnimal == null) return;

        _animationContext.PlayAnimation(_lastAnimal.transform);
    }
    private void Rescue()
    {
        Vector3 mousePos = _input.GetMousePosition();
        bool collected = _hitDetector.TryHit(AnimalMask, mousePos);

        if (_save == null || _save.Data == null) return;
        if (_lastAnimal == null) return;

        if (!_lastAnimal.TryGetComponent<AnimalInstance>(out var inst))
        {
            Destroy(_lastAnimal);
            _lastAnimal = null;
            return;
        }

        var icon = GetRescuedIconById(inst.Id);

        var list = _save.Data.rescuedAnimalsIds;
        if (!list.Contains(inst.Id) && collected)
        {
            list.Add(inst.Id);
            _save.Save();

            _model.Rescue();
            if (icon != null && inst.Render != null)
                inst.Render.sprite = icon;

            _pickUpAnimation.PlayCollectAnimation(_lastAnimal.transform.position, ScoresConst.DEFAULT);
            _pickUpAnimation.PlayCenterToDeliverAnimation(icon, onDelivered: () => { Debug.Log("Animals"); });

            _lastAnimal = null;
        }
    }
    private Sprite GetRescuedIconById(int id)
    {
        for (int i = 0; i < _animalsData.Length; i++)
        {
            if (_animalsData[i].PersistentId == id)
                return _animalsData[i].ChangeIcon != null ? _animalsData[i].ChangeIcon : _animalsData[i].Icon;
        }
        return null;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _input.Rescued -= Rescue;
        _input.Help -= Help;
    }
}
