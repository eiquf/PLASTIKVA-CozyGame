using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public sealed class UI : MonoBehaviour
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
    [SerializeField] private Transform _buttonsParent;

    private readonly AnimationContext<Transform> _animationContext = new();
    private readonly ButtonsPopUpAnimation _anim = new();
    private bool _isOpen = false;

    [SerializeField] List<AudioSource> _sources = new();
    private bool _isShut = false;

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

    [Inject] private ISaveService _save;
    public void Initialize() => _animationContext.SetAnimationStrategy(_anim);

    //just lazy code ok
    public void Preferences()
    {
        _isOpen = !_isOpen;
        _anim.SetBool(_isOpen);

        _animationContext.PlayAnimation(_buttonsParent);
    }
    public void ClearData()
    {
        _save.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame() => Application.Quit();
    public void Sound()
    {
        _isShut = !_isShut;

        foreach (var item in _sources)
        {
            if (_isShut == true)
                item.volume = 0;
            else item.volume = 1;
        }
    }
}

