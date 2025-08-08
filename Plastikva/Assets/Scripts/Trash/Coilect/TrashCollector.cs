using R3;
using UnityEngine;
using Zenject;

public class TrashCollector : MonoBehaviour
{
    private TrashInputHandler _input;

    private TrashView _view;
    private TrashCollectorModel _model;

    private LevelUnlocking _levelUnlocking;

    private TrashLevelDef _currentLevel;

    private readonly LayerMask TrashMask = 1 << 6;
    private readonly LayerMask WallMask = 1 << 7;

    private readonly CompositeDisposable _disposables = new();
    private ReactiveProperty<int> _count = new();

    [Inject]
    private void Container(LevelUnlocking levelUnlocking, TrashInputHandler input, TrashView view)
    {
        _input = input;
        _levelUnlocking = levelUnlocking;
        _view = view;
    }
    public void Initialize()
    {
        _input.LeftMouseClicked += Collect;

        _model = new();
        _count = new ReactiveProperty<int>(1);

        _levelUnlocking.CurrentLevel
              .Subscribe(level =>
              {
                  _currentLevel = level;
                  _count.Value = _currentLevel.RequiredTrashCount;
                  _model.UpdateGoal(_count.Value);
                  _view.SetMaxCount(_count.Value);
              })
              .AddTo(_disposables);

        _model.CurrentCount
            .Subscribe(count => _view.SetCurrentCount(count))
            .AddTo(_disposables);

        _model.AllCollected
            .Where(collected => collected)
            .Subscribe(_ => Debug.Log("Do here tick for level unlock"))
            .AddTo(_disposables);
    }
    private void Collect()
    {
        Vector2 mousePos = _input.GetMousePosition();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out _, Mathf.Infinity, WallMask))
            return;

        if (Physics.Raycast(ray, out RaycastHit trashHit, Mathf.Infinity, TrashMask))
        {
            Vector3 trashPos = trashHit.point;
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 direction = (trashPos - cameraPos).normalized;
            float distance = Vector3.Distance(cameraPos, trashPos);

            if (!Physics.Raycast(cameraPos, direction, distance, WallMask))
            {
                Destroy(trashHit.transform.gameObject);
                _model.Collect();
            }
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
        _input.LeftMouseClicked -= Collect;
    }
}