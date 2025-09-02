using R3;
using Zenject;

public class AnimalsRescueModel : System.IDisposable
{
    private int _maxCount;

    private readonly ReactiveProperty<int> _currentCount = new(0);
    public Observable<int> CurrentCount => _currentCount;

    private readonly ReactiveProperty<bool> _allCollected = new(false);
    public Observable<bool> AllCollected => _allCollected;

    private ISaveService _save;

    [Inject]
    public void Construct(ISaveService save) => _save = save;

    public void Setup()
    {
        _currentCount.Value = _save.Data.animalsCount;
        _allCollected.Value = false;
    }

    public void UpdateGoal(int maxCount, bool resetProgress = true)
    {
        _maxCount = maxCount;

        if (resetProgress)
        {
            _currentCount.Value = 0;
            _save.Data.animalsCount = 0;
            _allCollected.Value = false;
            _save.Save();
        }
        else
        {
            _currentCount.Value = _save.Data.animalsCount;
            _allCollected.Value = _currentCount.Value >= _maxCount && _maxCount > 0;
        }
    }

    public void Rescue()
    {
        if (_maxCount <= 0 || _allCollected.Value) return;

        _currentCount.Value++;

        if (_currentCount.Value >= _maxCount)
        {
            _currentCount.Value = _maxCount;
            _allCollected.Value = true;
        }

        _save.Data.animalsCount = _currentCount.Value;
        _save.Save();
    }

    public void Dispose()
    {
        _currentCount.Dispose();
        _allCollected.Dispose();
    }
}
