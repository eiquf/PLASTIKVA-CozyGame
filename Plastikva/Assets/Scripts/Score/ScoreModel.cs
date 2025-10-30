using R3;

public class ScoreModel
{

    private readonly ReactiveProperty<int> _score = new(0);
    public Observable<int> Score => _score;

    private ISaveService _save;

    public void Setup(ISaveService save)
    {
        _save = save;   

        if (_save.Data.isFirstLaunch)
        {
            _save.Data.currentScore = ScoresConst.START_SCORE;
            _score.Value = ScoresConst.START_SCORE;
        }
        else _score.Value = _save.Data.currentScore;
    }
    public void UpdateCount(int addScore)
    {
        if (_score.Value < ScoresConst.MAX_SCORE)
        {
            _score.Value += addScore;
            _save.Data.currentScore = _score.Value;
        }
        else return;
    }
}