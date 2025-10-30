using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelUnlocking : MonoBehaviour
{
#if UNITY_EDITOR
    public bool finished;
#endif

    [SerializeField] private TrashLevelSet _levelSet;

    private ISaveService _save;

    private readonly MapView _mapView = new();

    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    private readonly ReactiveProperty<bool> _isTrashSort = new(false);
    public Observable<bool> IsTrashSortLevel => _isTrashSort;

    private readonly ReactiveProperty<bool> _isTrashCollected = new(false);
    private readonly ReactiveProperty<bool> _isAnimalRescued = new(false);

    private readonly CompositeDisposable _disposables = new();

    private UI _ui;

    [Inject]
    public void Container(UI ui) => _ui = ui;

    public void Initialize(ISaveService save)
    {
        _save = save;

        _mapView.SetUp(_ui);

        if (_save.Data.isFirstLaunch)
        {
            _save.Data.currentLevelIndex = 0;
            _save.Data.currentEnvironment = EnvironmentType.Sewerage;
            _save.Data.isFirstLaunch = false;
            _save.Data.wallsIds ??= new List<int>();

            if (_save.Data.wallsIds.Count < 2)
            {
                _save.Data.wallsIds.Clear();
                _save.Data.wallsIds.Add(0);
                _save.Data.wallsIds.Add(1);
            }
        }

        if (_levelSet == null || _levelSet.Levels == null || _levelSet.Levels.Length == 0)
            return;

        _save.Data.currentLevelIndex = Mathf.Clamp(_save.Data.currentLevelIndex, 0, _levelSet.Levels.Length - 1);

        _currentLevel.Value = _levelSet.Levels[_save.Data.currentLevelIndex];

        _isTrashCollected.Value = false;
        _isAnimalRescued.Value = false;

        SaveGameData();

        ChangeStates();

        Observable
            .CombineLatest(_isTrashCollected, _isAnimalRescued, (t, a) => t && a)
            .DistinctUntilChanged()
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

    private void SaveGameData()
    {
        _save.Data.currentLevelIndex = Array.IndexOf(_levelSet.Levels, _currentLevel.Value);
    }

    private void UnlockLevel()
    {
        var nextIdx = Mathf.Clamp(_save.Data.currentLevelIndex + 1, 0, _levelSet.Levels.Length - 1);

        if (_save.Data.currentLevelIndex >= _levelSet.Levels.Length - 1)
        {
            HandleLastLevelCompleted();
            return;
        }

        if (nextIdx != _save.Data.currentLevelIndex)
        {
            AdvanceWallPair();

            _save.Data.currentLevelIndex = nextIdx;
            _currentLevel.Value = _levelSet.Levels[nextIdx];

            _mapView.UpdateView(_save.Data.currentLevelIndex);

            _isAnimalRescued.Value = false;
            _isTrashCollected.Value = false;

            _save.Data.collectedTrashIds?.Clear();

            SaveGameData();
        }
    }

    private void ChangeStates()
    {
        _isAnimalRescued
            .Subscribe(value =>
            {
                _save.Data.isAnimalRescued = value;
                SaveGameData();
            })
            .AddTo(_disposables);

        _isTrashCollected
            .Subscribe(value =>
            {
                _save.Data.isTrashCollected = value;
                SaveGameData();
            })
            .AddTo(_disposables);
    }

    private void AdvanceWallPair()
    {
        int nextFirst = _save.Data.wallsIds[1];
        int nextSecond = nextFirst + 1;

        _save.Data.wallsIds[0] = nextFirst;
        _save.Data.wallsIds[1] = nextSecond;

        Debug.Log($"{_save.Data.wallsIds[0]} and {_save.Data.wallsIds[1]}");
    }

    private void HandleLastLevelCompleted()
    {
        _ui.LOL.SetActive(true);
    }
    private void OnDestroy() => _disposables?.Dispose();
}
