using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI : MonoBehaviour
{
    [Header("Trash Sort UI")]
    [SerializeField] private Image _trashImage;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private TextMeshProUGUI _finalText;

    [Header("Score UI")]
    [SerializeField] private Image _scoreBar;

    [Header("Counts")]
    [SerializeField] private TextMeshProUGUI _trashCountText;
    [SerializeField] private TextMeshProUGUI _animalsCountText;

    public Image TrashImage => _trashImage;
    public Button YesButton => _yesButton;
    public Button NoButton => _noButton;
    public TextMeshProUGUI FinalText => _finalText;
    public Image ScoreBar => _scoreBar;
    public TextMeshProUGUI TrashCountText => _trashCountText; 
    public TextMeshProUGUI AnimalsCountText => _animalsCountText;

}
