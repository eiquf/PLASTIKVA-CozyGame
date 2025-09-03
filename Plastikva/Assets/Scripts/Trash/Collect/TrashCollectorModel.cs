using R3;
using System;

public class TrashCollectorModel : IDisposable
{
    private int _maxCount;

    private readonly ReactiveProperty<int> _currentCount = new(0);
    public Observable<int> CurrentCount => _currentCount;

    private readonly ReactiveProperty<bool> _allCollected = new(false);
    public Observable<bool> AllCollected => _allCollected;

    private ISaveService _save;
    public void Setup(ISaveService save) => _save = save;
    public void UpdateGoal(int maxCount, bool resetProgress = true)
    {
        _maxCount = maxCount;
        if (resetProgress)
        {
            _currentCount.Value = 0;
            _save.Data.trashCount = 0;
            _allCollected.Value = false;
        }
        else
        {
            _currentCount.Value = _save.Data.trashCount;
            _allCollected.Value = _currentCount.Value >= _maxCount && _maxCount > 0;
        }
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

            _save.Data.trashCount = _currentCount.Value;
        }
    }
    public void Dispose()
    {
        _currentCount.Dispose();
        _allCollected.Dispose();
    }
}
