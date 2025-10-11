using R3;
using UnityEngine;
using Zenject;

public class SharksMovement : MonoBehaviour
{
    [SerializeField] private BoxCollider _plane;
    [SerializeField] private Transform[] _walls;
    [SerializeField] private Transform _shark;
    [SerializeField] private SpriteRenderer sr;

    private readonly float[] _bounds = new float[2];

    [Header("Switching At Waypoints/Player")]
    [SerializeField] private float switchDistance = 0.3f;
    [SerializeField] private float switchDelay = 1f;
    private float switchTimer = 0f;

    [Header("Chase Resume When Far")]
    [SerializeField] private float farDistance = 8f;
    [SerializeField] private float returnDelay = 5f;
    private float timeFar = 0f;

    [Header("Movement")]
    public Transform _player;
    public float speed = 3f;
    public float zigzagAmplitude = 0.2f;
    public float zigzagFrequency = 2f;
    public float desyncDelay = 0.5f;

    [SerializeField] private LayerMask obstacleMask;
    [Header("Avoidance")]
    [SerializeField] private float avoidDistance = 1.5f;     
    [SerializeField] private float sideProbeDistance = 1.2f;   
    [SerializeField] private float avoidWeight = 2.0f;       
    [SerializeField] private float sharkRadius = 0.4f;      
    [SerializeField] private float maxSteerAngle = 45f;

    private Vector3 startPos;
    private bool movingToPlayer = true;
    private float offsetTime;

    private LevelUnlocking _unlocking;
    private IsometricCamera _camera;
    private ISaveService _save;
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking) => _unlocking = unlocking;

    public void Initialize(ISaveService save, IsometricCamera camera)
    {
        _save = save;
        _camera = camera;

        Bounds bounds = _plane.bounds;
        _bounds[0] = bounds.min.z;
        _bounds[1] = bounds.max.z;

        offsetTime = Random.Range(0f, desyncDelay);

        _unlocking.CurrentLevel
            .Subscribe(level => { startPos = GetRandomPosition(); })
            .AddTo(_disposables);

        _camera.IsBackSide
            .Subscribe(_ => { movingToPlayer = false; })
            .AddTo(_disposables);

        if (startPos == Vector3.zero)
            startPos = GetRandomPosition();
    }

    void FixedUpdate()
    {
        if (_player == null || _shark == null) return;

        float swimY = Mathf.Sin((Time.time + offsetTime) * zigzagFrequency) * zigzagAmplitude;

        Vector3 target = movingToPlayer
            ? new Vector3(_player.position.x, _shark.position.y + swimY, _player.position.z)
            : new Vector3(startPos.x, _shark.position.y + swimY, startPos.z);

        _shark.position = Vector3.MoveTowards(_shark.position, target, speed * Time.deltaTime);

        if (sr != null)
            sr.flipX = (_player.position.x < _shark.position.x);

        float distanceToPlayer = Vector3.Distance(_shark.position, _player.position);
        float distanceToStart = Vector3.Distance(_shark.position, startPos);

        if (movingToPlayer && distanceToPlayer <= switchDistance)
        {
            switchTimer += Time.deltaTime;
            if (switchTimer >= switchDelay)
            {
                movingToPlayer = false;
                switchTimer = 0f;
                timeFar = 0f;
            }
        }
        else if (!movingToPlayer && distanceToStart <= switchDistance)
        {
            switchTimer += Time.deltaTime;
            if (switchTimer >= switchDelay)
            {
                movingToPlayer = true;
                switchTimer = 0f;
                timeFar = 0f;
            }
        }
        else
            switchTimer = 0f;

        if (!movingToPlayer)
        {
            if (distanceToPlayer > farDistance)
            {
                timeFar += Time.deltaTime;
                if (timeFar >= returnDelay)
                {
                    movingToPlayer = true;
                    timeFar = 0f;
                }
            }
            else
                timeFar = 0f;
        }
        else
            timeFar = 0f; 
    }

    public void SetFollowTarget(Transform player) => _player = player;

    private Vector3 GetRandomPosition()
    {
        if (_walls == null || _walls.Length < 2 || _save == null || _save.Data == null || _save.Data.wallsIds.Count < 2)
            return Vector3.zero;

        int first = _save.Data.wallsIds[0];
        int second = _save.Data.wallsIds[1];

        first = Mathf.Clamp(first, 0, _walls.Length - 1);
        second = Mathf.Clamp(second, 0, _walls.Length - 1);

        float posX = (_walls[first].position.x + _walls[second].position.x) / 2f;
        float posZ = _bounds[Random.Range(0, 2)];

        return new Vector3(posX, _shark.position.y, posZ);
    }

    private void OnDestroy() => _disposables.Dispose();
}
