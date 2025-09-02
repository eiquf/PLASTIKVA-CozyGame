using R3;
using System;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour
{
    [SerializeField] private TrashLevelSet _levelSet;

    private GameData _levelData;
    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    private readonly ReactiveProperty<TrashLevelDef> _unlockedLevel = new();
    public Observable<TrashLevelDef> UnlockedLevel => _unlockedLevel;

    private readonly ReactiveProperty<bool> _isTrashSort = new(false);
    public Observable<bool> IsTrashSortLevel => _isTrashSort;

    private readonly ReactiveProperty<bool> _isTrashCollected = new(false);
    private readonly ReactiveProperty<bool> _isAnimalRescued = new(false);

    private readonly CompositeDisposable _disposables = new();

    public void Initialize()
    {
        //SaveLoadLevel.ClearSaveData();
        _levelData = SaveLoadLevel.Load<GameData>();


        if (_levelData.isFirstLaunch)
        {
            _levelData.currentLevelIndex = 0;
            _levelData.currentEnvironment = GameData.EnvironmentType.Sewerage;
            _levelData.isFirstLaunch = false;
        }

        if (_levelSet.Levels == null || _levelSet.Levels.Length == 0)
            return;

        _levelData.currentLevelIndex = Mathf.Clamp(_levelData.currentLevelIndex, 0, _levelSet.Levels.Length - 1);

        _currentLevel.Value = _levelSet.Levels[_levelData.currentLevelIndex];

        _isTrashCollected.Value = _levelData.isTrashCollected;
        _isAnimalRescued.Value = _levelData.isAnimalRescued;
        _isTrashSort.Value = _levelData.isTrashSorted;

        SaveGameData();

        ChangeStates();

        Observable
        .CombineLatest(_isTrashCollected, _isAnimalRescued, (t, a) => t && a)
        .DistinctUntilChanged()
        .Where(x => x)
        .Subscribe(value => _isTrashSort.Value = value)
        .AddTo(_disposables);
    }
    public void ReportTrashCollected() => _isTrashCollected.Value = true;
    public void ReportAnimalsRescued() => _isAnimalRescued.Value = true;
    private void SaveGameData()
    {
        _levelData.currentLevelIndex = Array.IndexOf(_levelSet.Levels, _currentLevel.Value);
        SaveLoadLevel.Save(_levelData);
    }
    private void UnlockLevel()
    {
       

        var nextIdx = Mathf.Clamp(_levelData.currentLevelIndex + 1, 0, _levelSet.Levels.Length - 1);

        if (nextIdx != _levelData.currentLevelIndex)
        {
            _levelData.currentLevelIndex = nextIdx;
            _currentLevel.Value = _levelSet.Levels[nextIdx];

            _isAnimalRescued.Value = false;
            _isTrashCollected.Value = false;
            _isTrashSort.Value = false;

            SaveGameData();
        }
    }
    private void ChangeStates()
    {
        _isAnimalRescued.Subscribe(value =>
        {
            _levelData.isAnimalRescued = value;
            SaveGameData();
        });


        _isTrashCollected.Subscribe(value =>
        {
            _levelData.isTrashCollected = value;
            SaveGameData();
        });

        _isTrashSort.Subscribe(value =>
        {
            _levelData.isTrashSorted = value;
            SaveGameData();
        });
    }
    private void OnDestroy() => _disposables?.Dispose();
}