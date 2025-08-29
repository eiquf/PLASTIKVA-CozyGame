using R3;
using UnityEngine;
using Zenject;


public sealed class TrashSorter : MonoBehaviour, IScore
{
    private readonly TrashSortModel _model = new();
    private TrashSortView _view;

   private LevelUnlocking _unlock;
    private UI _ui;

    private readonly CompositeDisposable _disposables = new();

    public ReactiveCommand TakenCommand { get; } = new ReactiveCommand();

    [Inject]
    public void Container(LevelUnlocking unlocking, TrashSortView view, UI ui)
    {
        _unlock = unlocking;
        _view = view;
        _ui = ui;
    }
    public void Initialize()
    {
        _view.SetUp(_ui);

        _unlock.CurrentLevel
               .Subscribe(level =>
               {
                   if (level == null || level.Trash == null || level.Trash.Length == 0)
                   {
                       _model.SetData(System.Array.Empty<TrashData>());
                       _view.SetButtonsInteractable(false);
                       _view.SetSprite(null);
                       return;
                   }

                   _model.SetData(level.Trash);
                   _view.SetButtonsInteractable(true);
               })
               .AddTo(_disposables);

        _view.YesClicks
            .Subscribe(_ =>
            {
                _model.Submit(true);
                TakenCommand.Execute(Unit.Default);
            })
            .AddTo(_disposables);

        _view.NoClicks.Subscribe(_ => _model.Submit(false)).AddTo(_disposables);

        _model.OnSprite.Subscribe(sprite =>
        {
            _view.SetSprite(sprite);
            if (sprite == null)
                _view.SetButtonsInteractable(false);
        }).AddTo(_disposables);
    }

    private void OnDestroy() => _disposables.Dispose();
}
