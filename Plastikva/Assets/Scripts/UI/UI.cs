using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject Star => _coin;
    public Transform StarsPos => _starsPos;
    public GameObject LOL => _LOL;
    public RectTransform CenterAnchor => _centerAnchor;
}
