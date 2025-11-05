using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialSystem : MonoBehaviour
{
    private enum Index
    {
        Movement = 0, Camera = 1, Tap = 2, Enemy = 3
    }

    private readonly TutorInputHandler _tutorInput;

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
        _save = save;

        _moveTutor = new MoveTutor(_moveTutorialUI[(int)Index.Movement], _save, _tutorInput);

        _checkInterval = Mathf.Max(0.1f, 0.5f);
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer < _tutorialDelay) return;

        _checkTimer += Time.deltaTime;


        if (_checkTimer >= _checkInterval)
        {
            _checkTimer = 0f;

            _moveTutor.Execute();
        }

    }
}

public abstract class Tutorial
{
    protected GameObject tutor;
    protected ISaveService save;
    protected TutorInputHandler input;
    public Tutorial(GameObject tutorial, ISaveService service, TutorInputHandler handler)
    {
        tutor = tutorial;
        save = service;
        input = handler;
    }
    public abstract void Execute();
}
public class MoveTutor : Tutorial
{
    public MoveTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler) : base(tutorial, service, handler) { }
    public override void Execute()
    {
        if (save.Data.isMovementTaught == false)
        {
            tutor.SetActive(true);
            if (input.MovementInput().sqrMagnitude > 0.01f)
                save.Data.isMovementTaught = true;
        }
        else
        {
            tutor.SetActive(false);
            save.Data.isMovementTaught = true;
        }
    }
}
public class CameraTutor : Tutorial
{
    public CameraTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler) : base(tutorial, service, handler) { }
    public override void Execute()
    {
        if (save.Data.isCameraTaught == false)
        {
            tutor.SetActive(true);
            if (input.CameraInput().sqrMagnitude > 0.01f)
                save.Data.isCameraTaught = true;
        }
        else
        {
            tutor.SetActive(false);
            save.Data.isCameraTaught = true;
        }
    }
}

public class TutorInputHandler
{
    private readonly InputController _inputController;
    [Inject]
    public TutorInputHandler(InputController inputController) => _inputController = inputController;
    public Vector2 MovementInput() => _inputController.Input.Gameplay.Move.ReadValue<Vector2>();
    public Vector2 CameraInput() => _inputController.Input.Camera.MouseDelta.ReadValue<Vector2>();
}