using R3;

public class ScoreModel
{
    private const int START_SCORE = 0;
    private const int UPDATE_SCORE = 1;
    private const int MAX_SCORE = 100;

    private readonly ReactiveProperty<int> _score = new(0);
    public Observable<int> Score => _score;

    private GameData _levelData;

    public void Setup()
    {
        _levelData = SaveLoadLevel.Load<GameData>();

        if (_levelData.isFirstLaunch)
        {
            _levelData.currentScore = START_SCORE;
            _score.Value = START_SCORE;
        }
        else _score.Value = _levelData.currentScore;

        SaveData();
    }
    public void UpdateCount()
    {
        if (_score.Value < MAX_SCORE)
        {
            _score.Value += UPDATE_SCORE;
            _levelData.currentScore = _score.Value;
        }
        else return;

        SaveData();
    }
    private void SaveData() => SaveLoadLevel.Save(_levelData);
}
