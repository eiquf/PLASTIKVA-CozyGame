using R3;
using System;

public class TrashCollectorModel : IDisposable
{
    private int _maxCount;

    private readonly ReactiveProperty<int> _currentCount = new(0);
    public Observable<int> CurrentCount => _currentCount;

    private readonly ReactiveProperty<bool> _allCollected = new(false);
    public Observable<bool> AllCollected => _allCollected;

    public TrashCollectorModel()
    {
        _currentCount = new ReactiveProperty<int>(0);
        _allCollected = new ReactiveProperty<bool>(false);
    }

    public void UpdateGoal(int maxCount)
    {
        _maxCount = maxCount;
        _currentCount.Value = 0;
        _allCollected.Value = false;
    }
    public void Collect()
    {
        if (_maxCount <= 0) return;

        if (!_allCollected.Value)
        {
            _currentCount.Value++;

            if (_currentCount.Value >= _maxCount)
            {
                _currentCount.Value = _maxCount;
                _allCollected.Value = true;
            }
        }
    }

    public void Dispose()
    {
        _currentCount.Dispose();
        _allCollected.Dispose();
    }
}
