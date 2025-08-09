using R3;

public class TrashView
{
    private readonly ReactiveProperty<int> _currentCount = new();
    public Observable<int> CurrentCount => _currentCount;

    private int _maxCount;
    public int MaxCount => _maxCount;

    public void SetMaxCount(int count) => _maxCount = count;
    public void SetCurrentCount(int count)
    {
        if (_currentCount.Value != count)
            _currentCount.Value = count;
    }
}