using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnimalsMovement : MonoBehaviour
{
    [SerializeField] private Transform _pos;
    [SerializeField] private Transform[] _walls;

    private readonly List<Transform> _animalsPos = new();
    private readonly List<AnimalInstance> _animals = new();

    private readonly MovementAnimator _anim = new();
    private LevelUnlocking _unlocking;
    private readonly CompositeDisposable _disposables = new();

    private readonly List<Vector3> _targetPositions = new();

    private ISaveService _save;

    [Inject]
    private void Container(LevelUnlocking unlocking) => _unlocking = unlocking;

    public void Initialize(ISaveService save)
    {
        _save = save;

        _save.Data.wallsIds ??= new List<int>();

        if (_save.Data.wallsIds.Count < 2)
        {
            _save.Data.wallsIds.Clear();
            _save.Data.wallsIds.Add(0);
            _save.Data.wallsIds.Add(1);
        }

        _unlocking.CurrentLevel
            .Subscribe(level =>
            {
                if (level == null) return;

                _animalsPos.Clear();
                _animals.Clear();
                _targetPositions.Clear();

                foreach (var animal in _pos.GetComponentsInChildren<AnimalInstance>())
                {
                    if (animal == null || animal == _pos) continue;

                    _targetPositions.Add(GetRandomWallPositionInPair());

                    if (!animal.IsStatic)
                    {
                        _animals.Add(animal);
                        _animalsPos.Add(animal.transform);
                    }
                }


                Debug.Log(_save.Data.wallsIds[0] + "and" + _save.Data.wallsIds[1]);
            })
            .AddTo(_disposables);

        _unlocking.CurrentLevel
            .Skip(1)
            .Subscribe(level => { AdvanceWallPair(); Debug.Log("AdvanceWallPair called"); }).AddTo(_disposables);
    }

    private void FixedUpdate()
    {
        if (_animalsPos.Count == 0 || _animals.Count == 0)
            return;

        for (int i = 0; i < _animalsPos.Count; i++)
        {
            Transform animal = _animalsPos[i];
            AnimalInstance instance = _animals[i];
            SpriteRenderer renderer = instance.Render;
            Vector3 target = _targetPositions[i];

            bool reached = _anim.MoveTo(target, animal);

            Vector3 direction = target - animal.position;
            bool movingRight = direction.x < 0f;

            renderer.flipX = !movingRight;

            if (reached)
                _targetPositions[i] = GetRandomWallPositionInPair();
        }
    }
    private Vector3 GetRandomWallPositionInPair()
    {
        if (_walls == null || _walls.Length < 2)
            return Vector3.zero;

        int first = _save.Data.wallsIds[0];
        int second = _save.Data.wallsIds[1];

        int index = Random.value < 0.5f ? first : second;
        return _walls[index].position;
    }
    private void AdvanceWallPair()
    {
        int nextFirst = _save.Data.wallsIds[1];
        int nextSecond = nextFirst + 1;

        _save.Data.wallsIds[0] = nextFirst;
        _save.Data.wallsIds[1] = nextSecond;

        Debug.Log($"{_save.Data.wallsIds[0]} and {_save.Data.wallsIds[1]}");
    }

    private void OnDestroy() => _disposables.Dispose();
}
