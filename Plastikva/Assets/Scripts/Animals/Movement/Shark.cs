using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Shark : MonoBehaviour, IEnemyContext, IScore
{
    private List<Transform> _walls = new();

    private readonly float[] _bounds = new float[2];

    private Vector3 _startPos;
    private Transform _followTarget;

    public float SwitchDistance => 0.3f;
    public float SwitchDelay => 1f;
    public float FarDistance => 8f;
    public float ReturnDelay => 5f;
    public float Speed => 3f;
    public float ZigzagAmplitude => 0.2f;
    public float ZigzagFrequency => 2f;
    public float DesyncDelay => 0.5f;

    public LayerMask ObstacleMask => 7;
    public LayerMask PlayerMask => 10;
    public float AvoidDistance => 1.5f;
    public float SideProbeDistance => 1.2f;
    public float AvoidWeight => 2f;
    public float EnemyRadius => 0.4f;
    public float MaxSteerAngle => 45;

    public Transform FollowTarget => _followTarget;
    public Transform Pos => transform;

    public bool MoveToPlayer => _movingToPlayer;

    public ReactiveCommand<int> TakenCommand { get; } = new ReactiveCommand<int>();

    private bool _movingToPlayer = false;

    private readonly EnemyMovement _movement = new();

    private LevelUnlocking _unlocking;
    private IsometricCamera _camera;
    private ISaveService _save;
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking) => _unlocking = unlocking;
    public void Initialize(ISaveService save, IsometricCamera camera, BoxCollider plane, List<Transform> walls)
    {
        _save = save;
        _camera = camera;

        Bounds bounds = plane.bounds;
        _bounds[0] = bounds.min.z;
        _bounds[1] = bounds.max.z;

        _walls = walls;

        _unlocking.CurrentLevel
            .Subscribe(level => { _startPos = GetRandomPosition(); })
            .AddTo(_disposables);

        _camera.IsBackSide
            .Subscribe(_ => { 
                _movingToPlayer = false; 
                _movement.UpdateMoving(); 
            })
            .AddTo(_disposables);


        Vector3 direction = (_followTarget.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _followTarget.position) + 1f; // Add some buffer

        Debug.Log("Raycast Direction: " + direction);
        Debug.Log("Raycast Distance: " + distance);

        Debug.DrawRay(transform.position, direction * distance, Color.red, 1f); // This will show the ray in the editor

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, ObstacleMask | PlayerMask))
        {
            Debug.Log("Hit something: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }


        if (Physics.Raycast(transform.position, direction, out RaycastHit hitt, distance, ObstacleMask | PlayerMask)){
            if (hitt.collider.gameObject.layer == LayerMask.NameToLayer("Player")) _movingToPlayer = true;
            else _movingToPlayer = false;

            Debug.Log(hitt.collider.gameObject.layer);
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }

        if (_startPos == Vector3.zero)
            _startPos = GetRandomPosition();

        _movement.Initialize(_startPos, this);
    }
    private void FixedUpdate()
    {
        _movement.Execute();
        if(transform.position.x == _followTarget.position.x) TakenCommand.Execute(-ScoresConst.DEFAULT);
    }
    private Vector3 GetRandomPosition()
    {
        if (_walls == null || _walls.Count < 2 || _save == null || _save.Data == null || _save.Data.wallsIds.Count < 2)
            return Vector3.zero;

        int first = _save.Data.wallsIds[0];
        int second = _save.Data.wallsIds[1];

        first = Mathf.Clamp(first, 0, _walls.Count - 1);
        second = Mathf.Clamp(second, 0, _walls.Count - 1);

        float posX = (_walls[first].position.x + _walls[second].position.x) / 2f;
        float posZ = _bounds[Random.Range(0, 2)];

        return new Vector3(posX, transform.position.y, posZ);
    }
    public void SetFollowTarget(Transform player) => _followTarget = player;
    private void OnDestroy() => _disposables.Dispose();
}
