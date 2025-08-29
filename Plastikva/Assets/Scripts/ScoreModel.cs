using R3;

public class ScoreModel
{
    private readonly ReactiveProperty<int> _score = new(0);
    public Observable<int> Score => _score;

    private int _updateCount;
    public void SetParametres(int currentCount, int updateCount)
    {
        _score.Value = currentCount;
        _updateCount = updateCount;
    }
    public void UpdateCount() => _score.Value += _updateCount;
}
