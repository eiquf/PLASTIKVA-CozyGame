using UnityEngine;
using Zenject;

public class Bootstrapper : MonoBehaviour
{
    [Inject] readonly DiContainer _diContainer;

    [SerializeField] private Player _playerPrefab;
    [SerializeField] private IsometricCamera _cameraPrefab;
    [SerializeField] private LevelUnlocking _levelUnlockingPref;
    [SerializeField] private TrashCollector _trashCollectorPrefab;
    [SerializeField] private UI _uiPrefab;

    private LevelUnlocking _levelUnlocking;
    private Player _playerInstance;
    private IsometricCamera _cameraInstance;
    private UI _uiInstance;
    private TrashCollector _trashCollector;

    private GameData _data;
    private void Start()
    {
        _data = SaveLoadLevel.Load<GameData>() ?? new GameData();
        SaveLoadLevel.Save(_data);

        _playerInstance = _diContainer.InstantiatePrefab(_playerPrefab).GetComponent<Player>();
        _cameraInstance = _diContainer.InstantiatePrefab(_cameraPrefab).GetComponent<IsometricCamera>();
        _uiInstance = _diContainer.InstantiatePrefab(_uiPrefab).GetComponent<UI>();
        _levelUnlocking = _diContainer.InstantiatePrefab(_levelUnlockingPref).GetComponent<LevelUnlocking>();

        _levelUnlocking.Initialize();
        _diContainer.Bind<LevelUnlocking>().FromInstance(_levelUnlocking).AsSingle();

        _trashCollector = _diContainer.InstantiatePrefab(_trashCollectorPrefab).GetComponent<TrashCollector>();

        _playerInstance.Initialize();
        _cameraInstance.Initialize();

        _cameraInstance.SetFollowTarget(_playerInstance.transform);
        _cameraInstance.SetBoundraries(_playerInstance.Boundry);

        _uiInstance.Initialize();

        _trashCollector.Initialize();
    }
}
