using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Score : MonoBehaviour
{
    private IReadOnlyList<IScore> _scores;

    private UI _ui;
    private readonly ScoreModel _model = new();
    private ScoreView _scoreView;
    private ISaveService _save;

    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(ScoreView view) => _scoreView = view;
    public void Initialize(List<IScore> scores, ISaveService save)
    {
        _save = save;
        _scores = scores;

        _ui = GetComponent<UI>();
        _scoreView.SetUp(_ui);
        _model.Setup(_save);

        _model.Score
              .Subscribe(score => _scoreView.Render(score))
              .AddTo(_disposables);

        foreach (var s in _scores)
        {
            s.TakenCommand
             .Subscribe(_ => _model.UpdateCount())
             .AddTo(_disposables);
        }
    }
    private void OnDestroy() => _disposables.Dispose();
}
