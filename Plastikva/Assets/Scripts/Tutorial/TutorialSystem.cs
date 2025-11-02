using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialSystem : MonoBehaviour
{
    private enum Index
    {
        Movement = 0, Camera = 1, Tap = 2, Enemy = 3
    }

    private TutorInputHandler _tutorInput;

    [SerializeField] private List<GameObject> _moveTutorialUI = new();

    private Tutorial _moveTutor;

    private readonly float _tutorialDelay = 2f;
    private float _timer;

    private float _checkTimer;
    private float _checkInterval = 1f;

    private bool _hasMoved;

    private ISaveService _save;

    public TutorialSystem(TutorInputHandler handler) => _tutorInput = handler;
    private void Initialize(ISaveService save)
    {
        //_save = save;

        //_moveTutor = new MoveTutor(_moveTutorialUI[(int)Index.Movement], _save);

        //_checkInterval = Mathf.Max(0.1f, 0.5f);
    }

    private void FixedUpdate()
    {
        //_timer += Time.deltaTime;

        //if (_timer < _tutorialDelay) return;

        //_checkTimer += Time.deltaTime;


        //if (_checkTimer >= _checkInterval)
        //{
        //    _checkTimer = 0f;

        //    if (_save.Data.isMovementTaught == false)
        //    {
        //        if (_tutorInput.MovementInput().sqrMagnitude > 0.01f)
        //            _save.Data.isMovementTaught = true;

        //        _moveTutor.Execute();
        //    }
        //}

    }
}

public abstract class Tutorial
{
    protected GameObject tutor;
    protected ISaveService saveService;
    public Tutorial(GameObject tutorial, ISaveService service)
    {
        tutor = tutorial;
        saveService = service;
    }
    public abstract void Execute();
}
public class MoveTutor : Tutorial
{

    public MoveTutor(GameObject tutorial, ISaveService service) : base(tutorial, service) { }
    public override void Execute()
    {
        tutor.SetActive(true);
        saveService.Data.isMovementTaught = true;
    }
}

public class TutorInputHandler
{
    private readonly InputController _inputController;
    [Inject]
    public TutorInputHandler(InputController inputController) => _inputController = inputController;
    public Vector2 MovementInput() => _inputController.Input.Gameplay.Move.ReadValue<Vector2>();
}