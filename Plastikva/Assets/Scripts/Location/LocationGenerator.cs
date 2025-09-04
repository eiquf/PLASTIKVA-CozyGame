using R3;
using TMPro;
using UnityEngine;
using Zenject;

public class LocationGenerator : MonoBehaviour
{
    private readonly LevelGeneratorModel _model = new();
    private readonly LevelGeneratorView _view  = new();

    private Transform _plane; 

    private LevelUnlocking _unlocking;
    private UI _ui;

    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking, UI ui)
    {
        _unlocking = unlocking;
        _ui = ui;
    }

    public void Initialize()
    {
        _view.Setup(_ui); 

        _unlocking.CurrentLevel
            .Subscribe(level =>
            {
                if (level == null || level.Trash == null || level.Trash.Length == 0)
                {
                    _model.Setup(System.Array.Empty<TrashData>(), _plane);
                    //_view.SetButtonsInteractable(false);
                    //_view.SetSprite(null);
                    return;
                }

                _view.ShowPanel();
                _model.Setup(level.Trash, _plane);
                //_view.SetButtonsInteractable(true);
            })
            .AddTo(_disposables);


    }
    private void OnDestroy() => _disposables.Dispose();
}

public class LevelGeneratorModel
{
    private Transform _plane;

    private TrashData[] _data = System.Array.Empty<TrashData>();

    public void Setup(TrashData[] data, Transform plane)
    {
        _plane = plane;
        _data = data ?? System.Array.Empty<TrashData>();
    }
    public void Generate()
    {
        float planeXSize = _plane.localScale.x * 10f;
        float planeZSize = _plane.localScale.z * 10f;

        Vector3 planeCenter = _plane.position;

        for (int i = 0; i < _data.Length; i++)
        {
            int index = i;

            float randomX = Random.Range(planeCenter.x - planeXSize / 2f, planeCenter.x + planeXSize / 2f);
            float randomZ = Random.Range(planeCenter.z - planeZSize / 2f, planeCenter.z + planeZSize / 2f);
            float spawnY = planeCenter.y;

            Vector3 randomSpawnPosition = new(randomX, spawnY, randomZ);

            Object.Instantiate(_data[index].Pref, randomSpawnPosition, Quaternion.identity);
        }
    }
}

public class LevelGeneratorView
{
    private TextMeshProUGUI _text;
    private CanvasGroup _canvasGroup;
    private readonly AnimationContext<CanvasGroup> _animContext = new();
    public void Setup(UI ui)
    {
        _canvasGroup = ui.CanvasGroup;
        _text = ui.LevelText;
        _animContext.SetAnimationStrategy(new ShowupAnimation());
    }
    public void ShowPanel()
    {
        _animContext.PlayAnimation(_canvasGroup);
    } 
}

public class ShowupAnimation : IAnimation<CanvasGroup>
{
    public void PlayAnimation(CanvasGroup hide)
    {
    }
}