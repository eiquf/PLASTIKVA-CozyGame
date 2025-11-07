using R3;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public sealed class UI : MonoBehaviour, ISound
{
    [Header("Trash Sort UI")]
    [SerializeField] private GameObject _panelSort;
    [SerializeField] private Image _trashImage;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private TextMeshProUGUI _finalText;
    [SerializeField] private CanvasGroup _sortCanvasGroup;

    [Header("Score UI")]
    [SerializeField] private Image _scoreBar;
    [SerializeField] private GameObject _coin;
    [SerializeField] private Transform _starsPos;

    [Header("Counts")]
    [SerializeField] private TextMeshProUGUI _trashCountText;
    [SerializeField] private TextMeshProUGUI _animalsCountText;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private CanvasGroup _levelCanvasGroup;

    [Header("Map")]
    [SerializeField] private RectTransform _fishPos;
    [SerializeField] private List<GameObject> _points;

    [SerializeField] private RectTransform _centerAnchor;

    [SerializeField] private GameObject _LOL;

    [SerializeField] private List<GameObject> _tutorialUI = new();

    //references
    [SerializeField] private Button _refButton;
    [SerializeField] private List<Sprite> _refButtonSprites;

    //sound
    [SerializeField] private Sprite[] _soundSprites = new Sprite[2];

    [SerializeField] private Transform _buttonsParent;

    private readonly AnimationContext<Transform> _animationContext = new();
    private readonly ButtonsPopUpAnimation _anim = new();
    private bool _isOpen = false;

    [SerializeField] private Button _soundButton;
    [SerializeField] private GameObject _arrows;
    public Image TrashImage => _trashImage;
    public Button YesButton => _yesButton;
    public Button NoButton => _noButton;
    public TextMeshProUGUI FinalText => _finalText;
    public Image ScoreBar => _scoreBar;
    public TextMeshProUGUI TrashCountText => _trashCountText;
    public TextMeshProUGUI AnimalsCountText => _animalsCountText;
    public GameObject PanelSort => _panelSort;
    public CanvasGroup CanvasGroup => _levelCanvasGroup;
    public CanvasGroup SortTextGroup => _sortCanvasGroup;
    public TextMeshProUGUI LevelText => _levelText;
    public RectTransform FishPos => _fishPos;
    public List<GameObject> Points => _points;
    public List<GameObject> Tutorials => _tutorialUI;
    public GameObject Star => _coin;
    public Transform StarsPos => _starsPos;
    public GameObject LOL => _LOL;
    public RectTransform CenterAnchor => _centerAnchor;
    public Sprite[] SoundSprites => _soundSprites;
    public Button SoundButton => _soundButton;

    public ReactiveCommand<int> PlayCommand { get; } = new ReactiveCommand<int>();

    [Inject] private ISaveService _save;

    public void Initialize()
    {
        _animationContext.SetAnimationStrategy(_anim);

        if (Application.isMobilePlatform) _arrows.SetActive(true);
        else _arrows.SetActive(false);

        Debug.Log("ASpplication ,obile" + Application.isMobilePlatform);
    }

    //just lazy code ok
    public void Preferences()
    {
        PlayCommand.Execute((int)GameSound.Shell);

        _isOpen = !_isOpen;

        if (_refButtonSprites.Count >= 2)
        {
            _refButton.image.sprite = _isOpen
                ? _refButtonSprites[1]
                : _refButtonSprites[0];
        }
        _anim.SetBool(_isOpen);

        _animationContext.PlayAnimation(_buttonsParent);
    }
    public void ClearData()
    {
        _save.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame() => Application.Quit();
}
