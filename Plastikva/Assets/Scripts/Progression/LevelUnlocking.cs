using R3;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour
{
    [SerializeField] private TrashLevelSet _levelSet;
    private readonly ReactiveProperty<TrashLevelDef> _currentLevel = new();
    public Observable<TrashLevelDef> CurrentLevel => _currentLevel;

    public void Initialize()
    {
        _currentLevel.Value = _levelSet.Levels[0];
    }
}
