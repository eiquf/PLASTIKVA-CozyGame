using R3;
using UnityEngine;
using Zenject;


public sealed class TrashSorter : MonoBehaviour, IScore
{
#if UNITY_EDITOR
    public bool finished;
#endif
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

#if UNITY_EDITOR
        if (finished == true)
        {
            _unlock.ReportTrashSorted();
        }
#endif
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

                   _view.ShowPanel(false);
                   _model.SetData(level.Trash);
                   _view.SetButtonsInteractable(true);
               })
               .AddTo(_disposables);

        _unlock.IsTrashSortLevel
            .Subscribe(show => { _view.ShowPanel(show); })
            .AddTo(_disposables);

        _model.OnCompleted
            .Where(done => done)
            .Subscribe(x =>
            {
                _unlock.ReportTrashSorted();
                _view.ShowPanel(false);
            })
            .AddTo(_disposables);

        _model.OnProgress
            .WithLatestFrom(_model.OnScore, (p, s) => (progress: p, score: s))
            .Where(x => x.progress.total > 0 && x.progress.current == x.progress.total)
            .Subscribe(x =>
            {
                bool allCorrect = x.score == x.progress.total;
                if (allCorrect)
                {
                    _unlock.ReportTrashSorted();
                    _view.ShowPanel(false);
                }
                else
                {
                    //можно крч сделать эффект текста или попа
                    // _view.ShowRetry(); 
                    // _model.SetData(_model.GetLastData()); 
                }
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
