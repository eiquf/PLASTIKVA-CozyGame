using R3;
using System;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour
{
    [SerializeField] private TrashLevelSet _levelSet;

    private LevelData _levelData;
    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    private readonly ReactiveProperty<bool> _isTrashCollected = new(false);
    private readonly ReactiveProperty<bool> _isAnimalRescued = new(false);

    private IDisposable _subUnlock;

    public void Initialize()
    {
        _levelData = SaveLoadLevel.Load<LevelData>();

        if (_levelData.isFirstLaunch)
        {
            _levelData.currentLevelIndex = 0;
            _levelData.currentEnvironment = LevelData.EnvironmentType.Sewerage;
            _levelData.isFirstLaunch = false;
        }

        if (_levelSet.Levels == null || _levelSet.Levels.Length == 0)
            return;

        _levelData.currentLevelIndex = Mathf.Clamp(_levelData.currentLevelIndex, 0, _levelSet.Levels.Length - 1);

        _currentLevel.Value = _levelSet.Levels[_levelData.currentLevelIndex];

        _isTrashCollected.Value = _levelData.isTrashCollected;
        _isAnimalRescued.Value = _levelData.isAnimalRescued;

        SaveLevelData();
        Debug.Log(_isTrashCollected.Value + "" + _isAnimalRescued.Value);

        ChangeStates();

        _subUnlock = Observable
            .CombineLatest(_isTrashCollected, _isAnimalRescued, (t, a) => t && a)
            .DistinctUntilChanged()
            .Where(x => x)
            .Subscribe(_ => UnlockLevel());
    }
    public void ReportTrashCollected() => _isTrashCollected.Value = true;
    public void ReportAnimalsRescued() => _isAnimalRescued.Value = true;
    private void SaveLevelData()
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

            SaveLevelData();
        }
    }
    private void ChangeStates()
    {
        _isAnimalRescued.Subscribe(value => { 
            _levelData.isAnimalRescued = value; 
            SaveLevelData(); });


        _isTrashCollected.Subscribe(value => {
            _levelData.isTrashCollected = value;
            SaveLevelData();
        });
    }
    private void OnDestroy()
    {
        _subUnlock?.Dispose();
    }
}