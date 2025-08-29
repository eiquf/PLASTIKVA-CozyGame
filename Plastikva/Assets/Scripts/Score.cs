using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Score : MonoBehaviour
{
    private const int MAX_SCORE = 100;
    private const int START_SCORE = 0;
    private const int UPDATE_SCORE = 5;

    private int _currentScore;

    private GameData _levelData;

    private List<IScore> _scores = new();

    private UI _ui;
    private readonly ScoreModel _model = new();
    private ScoreView _scoreView;

    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(ScoreView view)
    {
        _scoreView = view;
    }
    public void Initialize(List<IScore> scoresList)
    {
        _scores = scoresList;

        _ui = GetComponent<UI>();
        _scoreView.SetUp(_ui);

        _levelData = SaveLoadLevel.Load<GameData>();

        if (_levelData.isFirstLaunch)
        {
            _levelData.currentScore = START_SCORE;
            _currentScore = START_SCORE;
        }
        else _currentScore = _levelData.currentScore;

        SaveData();

        _model.SetParametres(_currentScore, UPDATE_SCORE);
        _scoreView.SetMax(MAX_SCORE);

        _scores[0].TakenCommand.Subscribe(count => _model.UpdateCount()).AddTo(_disposables);

        _model.Score.Subscribe(count => _scoreView.ChangeCount(count)).AddTo(_disposables);
    }
    private void SaveData() => SaveLoadLevel.Save(_levelData);
    private void OnDestroy() => _disposables.Dispose();
}
