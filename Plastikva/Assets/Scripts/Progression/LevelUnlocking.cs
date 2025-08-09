using R3;
using System;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour
{
    [SerializeField] private TrashLevelSet _levelSet;

    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    private readonly ReactiveProperty<bool> _isTrashCollected = new(false);
    private readonly ReactiveProperty<bool> _isAnimalRescued = new(false);

    private IDisposable _subUnlock;

    public void Initialize()
    {
        _currentLevel.Value = _levelSet.Levels[0];

        _subUnlock = Observable
            .CombineLatest(_isTrashCollected, _isAnimalRescued, (t, a) => t && a)
            .DistinctUntilChanged()
            .Where(x => x)
            .Subscribe(_ => UnlockLevel());
    }

    public void ReportTrashCollected() => _isTrashCollected.Value = true;
    public void ReportAnimalsRescued() => _isAnimalRescued.Value = true;

    private void OnDestroy() => _subUnlock?.Dispose();

    private void LoadSortOutLevel()
    {

    }
    private void UnlockLevel()
    {
        var idx = Array.IndexOf(_levelSet.Levels, _currentLevel.Value);
        var nextIdx = Mathf.Clamp(idx + 1, 0, _levelSet.Levels.Length - 1);
        _currentLevel.Value = _levelSet.Levels[nextIdx];

        _isTrashCollected.Value = false;
        _isAnimalRescued.Value = false;
    }
}
