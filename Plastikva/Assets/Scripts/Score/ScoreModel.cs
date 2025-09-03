using R3;

public class ScoreModel
{
    private const int START_SCORE = 0;
    private const int UPDATE_SCORE = 1;
    private const int MAX_SCORE = 100;

    private readonly ReactiveProperty<int> _score = new(0);
    public Observable<int> Score => _score;

    private ISaveService _save;

    public void Setup(ISaveService save)
    {
        _save = save;   

        if (_save.Data.isFirstLaunch)
        {
            _save.Data.currentScore = START_SCORE;
            _score.Value = START_SCORE;
        }
        else _score.Value = _save.Data.currentScore;
    }
    public void UpdateCount()
    {
        if (_score.Value < MAX_SCORE)
        {
            _score.Value += UPDATE_SCORE;
            _save.Data.currentScore = _score.Value;
        }
        else return;
    }
}
