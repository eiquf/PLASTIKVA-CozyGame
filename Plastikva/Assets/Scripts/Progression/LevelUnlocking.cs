using R3;
using System;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour
{
#if UNITY_EDITOR
    public bool finished;
#endif
    [SerializeField] private TrashLevelSet _levelSet;

    private ISaveService _save;

    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    private readonly ReactiveProperty<bool> _isTrashSort = new(false);
    public Observable<bool> IsTrashSortLevel => _isTrashSort;

    private readonly ReactiveProperty<bool> _isTrashCollected = new(false);
    private readonly ReactiveProperty<bool> _isAnimalRescued = new(false);

    private readonly CompositeDisposable _disposables = new();

    public void Initialize(ISaveService save)
    {
        _save = save;

        if (_save.Data.isFirstLaunch)
        {
            _save.Data.currentLevelIndex = 0;
            _save.Data.currentEnvironment = EnvironmentType.Sewerage;
            _save.Data.isFirstLaunch = false;
        }

        if (_levelSet.Levels == null || _levelSet.Levels.Length == 0)
            return;

        _save.Data.currentLevelIndex = Mathf.Clamp(_save.Data.currentLevelIndex, 0, _levelSet.Levels.Length - 1);

        _currentLevel.Value = _levelSet.Levels[_save.Data.currentLevelIndex];

        _isTrashCollected.Value = _save.Data.isTrashCollected;
        _isAnimalRescued.Value = _save.Data.isAnimalRescued;
        _isTrashSort.Value = _save.Data.isTrashSorted;

        SaveGameData();

        ChangeStates();

        Observable
        .CombineLatest(_isTrashCollected, _isAnimalRescued, (t, a) => t && a)
        .DistinctUntilChanged()
        .Where(x => x)
        .Subscribe(value => _isTrashSort.Value = value)
        .AddTo(_disposables);

#if UNITY_EDITOR
        if (finished == true)
        {
            UnlockLevel();
        }
#endif
    }
    public void ReportTrashCollected() => _isTrashCollected.Value = true;
    public void ReportAnimalsRescued() => _isAnimalRescued.Value = true;
    public void ReportTrashSorted() => UnlockLevel();
    private void SaveGameData() => _save.Data.currentLevelIndex = Array.IndexOf(_levelSet.Levels, _currentLevel.Value);
    private void UnlockLevel()
    {
        var nextIdx = Mathf.Clamp(_save.Data.currentLevelIndex + 1, 0, _levelSet.Levels.Length - 1);

        if (nextIdx != _save.Data.currentLevelIndex)
        {
            _save.Data.currentLevelIndex = nextIdx;
            _currentLevel.Value = _levelSet.Levels[nextIdx];

            _isAnimalRescued.Value = false;
            _isTrashCollected.Value = false;
            _isTrashSort.Value = false;

            _save.Data.collectedTrashIds?.Clear();

            SaveGameData();
        }
    }
    private void ChangeStates()
    {
        _isAnimalRescued.Subscribe(value =>
        {
            _save.Data.isAnimalRescued = value;
            SaveGameData();
        });

        _isTrashCollected.Subscribe(value =>
        {
            _save.Data.isTrashCollected = value;
            SaveGameData();
        });

        _isTrashSort.Subscribe(value =>
        {
            _save.Data.isTrashSorted = value;
            SaveGameData();
        });
    }
    private void OnDestroy() => _disposables?.Dispose();
}