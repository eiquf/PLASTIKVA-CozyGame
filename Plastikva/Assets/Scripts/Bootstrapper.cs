using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Bootstrapper : MonoBehaviour
{
    [Inject] readonly DiContainer _diContainer;

    [SerializeField] private Player _playerPrefab;
    [SerializeField] private LevelUnlocking _levelUnlockingPref;
    [SerializeField] private LocationGenerator _locationGenerator;
    [SerializeField] private TrashCollector _trashCollectorPref;
    [SerializeField] private UI _uiPrefab;
    [SerializeField] private IsometricCamera _cameraPrefab;

    private LevelUnlocking _levelUnlocking;
    private LocationGenerator _generator;
    private FrustumCulling _frustumCulling;

    private Player _playerInstance;
    private IsometricCamera _cameraInstance;

    private UI _uiInstance;
    private Score _score;

    private TrashCollector _trashCollector;
    private TrashSorter _trashSorter;

    private AnimalsMovement _animalsMove;
    private AnimalsRescue _animals;

    private SharksMovement _shark;

    private ISaveService _save;
    private readonly List<IScore> _scores = new();

    private void Start()
    {
        //_save.Clear();
        _save = new SaveService();
        _save.LoadOrCreate();

        _diContainer.Bind<ISaveService>().FromInstance(_save).AsSingle();

        _playerInstance = _diContainer.InstantiatePrefab(_playerPrefab).GetComponent<Player>();
        _cameraInstance = _diContainer.InstantiatePrefab(_cameraPrefab).GetComponent<IsometricCamera>();

        _uiInstance = _diContainer.InstantiatePrefab(_uiPrefab).GetComponent<UI>();
        _diContainer.Bind<UI>().FromInstance(_uiInstance).AsSingle();
        _score = _uiInstance.GetComponent<Score>();

        _levelUnlocking = _diContainer.InstantiatePrefab(_levelUnlockingPref).GetComponent<LevelUnlocking>();
        _diContainer.Bind<LevelUnlocking>().FromInstance(_levelUnlocking).AsSingle();

        _levelUnlocking.Initialize(_save);

        _generator = _diContainer.InstantiatePrefab(_locationGenerator).GetComponent<LocationGenerator>();
        _generator.Initialize(_save);
        _frustumCulling = _generator.GetComponent<FrustumCulling>();
        _animalsMove = _generator.GetComponent<AnimalsMovement>();
        _shark = _generator.GetComponent<SharksMovement>();

        _trashCollector = _diContainer.InstantiatePrefab(_trashCollectorPref).GetComponent<TrashCollector>();
        _trashSorter = _trashCollector.GetComponent<TrashSorter>();
        _animals = _trashCollector.GetComponent<AnimalsRescue>();

        _scores.Add(_trashCollector);
        _scores.Add(_trashSorter);
        _scores.Add(_animals);

        _cameraInstance.Initialize();
        _playerInstance.Initialize(_cameraInstance);

        _cameraInstance.SetFollowTarget(_playerInstance.transform);

        Initializing();

        _save.Save();
    }
#if UNITY_EDITOR
    public void ClearData()
    {
        _save.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
#endif
    private void Initializing()
    {
        _trashCollector.Initialize(_save);
        _trashSorter.Initialize();
        _animals.Initialize(_save);
        _score.Initialize(_scores, _save);
        _frustumCulling.Initialize(_cameraInstance);
        _animalsMove.Initialize(_save);
        _shark.Initialize(_save, _cameraInstance);
        _shark.SetFollowTarget(_playerInstance.transform);
    }
    //private void OnDestroy() => _save.Save();
}