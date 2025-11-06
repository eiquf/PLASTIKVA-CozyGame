using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialSystem : MonoBehaviour
{
    private enum Index { Movement = 0, Camera = 1, Tap = 2, Enemy = 3 }

    [SerializeField] private List<GameObject> _tutorialUI = new();
    protected const string REQUEST_COMMAND = "Next";

    private Tutorial _currentTutor;

    private Tutorial _moveTutor;
    private Tutorial _cameraTutor;
    private Tutorial _tapTutor;
    private Tutorial _enemyTutor;

    private float _tutorialDelay = 2f;
    private float _timer;
    private float _checkTimer;
    private float _checkInterval = 0.5f;

    private ISaveService _save;
    private TutorInputHandler _tutorInput;

    [Inject]
    public void Container(ISaveService save, TutorInputHandler handler)
    {
        _save = save;
        _tutorInput = handler;
    }

    public void Initialize(UI ui)
    {
        _tutorialUI = ui.Tutorials;

        _moveTutor = new MoveTutor(_tutorialUI[(int)Index.Movement], _save, _tutorInput);
        _cameraTutor = new CameraTutor(_tutorialUI[(int)Index.Camera], _save, _tutorInput);
        _tapTutor = new TapTutor(_tutorialUI[(int)Index.Tap], _save, _tutorInput);
        _enemyTutor = new EnemyTutor(_tutorialUI[(int)Index.Enemy], _save, _tutorInput);

        _moveTutor.SetNext(_cameraTutor);
        _cameraTutor.SetNext(_tapTutor);
        _tapTutor.SetNext(_enemyTutor);
        _enemyTutor.SetNext(null);

        _currentTutor = _moveTutor;
    }

    private void FixedUpdate()
    {
        _tutorInput.Update();

        _timer += Time.deltaTime;
        if (_timer < _tutorialDelay) return;

        _checkTimer += Time.deltaTime;
        if (_checkTimer < _checkInterval) return;
        _checkTimer = 0f;

        if (_currentTutor == null) return;

        _currentTutor.Handle(REQUEST_COMMAND);

        if (IsTutorCompleted(_currentTutor))
            _currentTutor = _currentTutor.NextHandler;
    }

    private bool IsTutorCompleted(Tutorial tutor)
    {
        if (tutor is MoveTutor && _save.Data.isMovementTaught) return true;
        if (tutor is CameraTutor && _save.Data.isCameraTaught) return true;
        if (tutor is TapTutor && _save.Data.isTapTaught) return true;
        if (tutor is EnemyTutor && _save.Data.isEnemyTaught) return true;
        return false;
    }
}
