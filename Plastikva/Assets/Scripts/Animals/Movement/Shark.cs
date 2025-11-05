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

    // You can keep hardcoded masks, but serialized is nicer in Inspector.
    public LayerMask ObstacleMask => 1 << 7;
    public LayerMask PlayerMask => 1 << 10;

    [SerializeField] private LayerMask _shellMask;

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

    [SerializeField] private float _detectRadius = 6f;
    [SerializeField] private float _losPadding = 0.1f;

    private readonly Collider[] _hits = new Collider[8];

    private bool _wasMovingToPlayer = false;
    [SerializeField] private float _scoreCooldown = 1.0f;
    private float _nextScoreTime = 0f;

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
            .Subscribe(_ =>
            {
                _movingToPlayer = false;
                _movement.UpdateMoving();
            })
            .AddTo(_disposables);

        if (_startPos == Vector3.zero)
            _startPos = GetRandomPosition();

        _movement.Initialize(_startPos, this);
    }

    private void FixedUpdate()
    {
        if (_followTarget == null)
        {
            _movingToPlayer = false;
            _movement.Execute();
            return;
        }

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            _detectRadius,
            _hits,
            PlayerMask,
            QueryTriggerInteraction.Ignore
        );

        bool seesPlayer = false;
        bool shellBlocking = false; 

        if (count > 0)
        {
            Vector3 toPlayer = _followTarget.position - transform.position;
            float dist = toPlayer.magnitude + _losPadding;
            Vector3 dir = toPlayer / Mathf.Max(dist, 0.0001f);
            int losMask = PlayerMask | ObstacleMask;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist, losMask, QueryTriggerInteraction.Ignore))
            {
                int playerLayer = LayerMask.NameToLayer("Player");
                if (hit.collider.gameObject.layer == playerLayer) seesPlayer = true;               
                else if (((1 << hit.collider.gameObject.layer) & ObstacleMask) != 0)
                {
                    shellBlocking = true;    
                    seesPlayer = false;
                }
                else seesPlayer = false;
            }

            Debug.DrawRay(transform.position, dir * dist,
                seesPlayer ? Color.green : (shellBlocking ? Color.yellow : Color.red),
                0f, false);
        }

        _movingToPlayer = seesPlayer;
        _movement.Execute();


        if (_wasMovingToPlayer && !_movingToPlayer && Time.time >= _nextScoreTime)
        {
            TakenCommand.Execute(-ScoresConst.DEFAULT);
            _nextScoreTime = Time.time + _scoreCooldown;
        }

        if (_movingToPlayer &&
            Mathf.Abs(transform.position.x - _followTarget.position.x) < 0.01f &&
            Time.time >= _nextScoreTime)
        {
            TakenCommand.Execute(-ScoresConst.DEFAULT);
            _nextScoreTime = Time.time + _scoreCooldown;
        }

        if (shellBlocking && Time.time >= _nextScoreTime)
        {
            TakenCommand.Execute(-ScoresConst.DEFAULT);
            _nextScoreTime = Time.time + _scoreCooldown;
        }

        _wasMovingToPlayer = _movingToPlayer;
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
