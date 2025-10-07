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

    public Transform _player;
    public float speed = 3f;
    public float zigzagAmplitude = 0.2f;
    public float zigzagFrequency = 2f;
    public float desyncDelay = 0.5f;

    private Vector3 startPos;
    private bool movingToPlayer = true;
    private float offsetTime;

    private LevelUnlocking _unlocking;
    private ISaveService _save;
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    private void Container(LevelUnlocking unlocking) => _unlocking = unlocking;

    public void Initialize(ISaveService save)
    {
        _save = save;

        Bounds bounds = _plane.bounds;
        _bounds[0] = bounds.min.z;
        _bounds[1] = bounds.max.z;

        offsetTime = Random.Range(0f, desyncDelay);

        _unlocking.CurrentLevel
            .Subscribe(level => { startPos = GetRandomPosition(); })
            .AddTo(_disposables);

        // fallback in case subscription doesn't fire immediately
        if (startPos == Vector3.zero)
            startPos = GetRandomPosition();
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        // плавное покачивание вверх-вниз (имитация плавания)
        float swimY = Mathf.Sin((Time.time + offsetTime) * zigzagFrequency) * zigzagAmplitude;

        Vector3 target;
        if (movingToPlayer)
        {
            // двигаемся к игроку по XZ, Y лишь колеблется
            target = new Vector3(_player.position.x, _shark.position.y + swimY, _player.position.z);
        }
        else
        {
            // возвращаемся в стартовую позицию
            target = new Vector3(startPos.x, _shark.position.y + swimY, startPos.z);
        }

        // движение к цели
        _shark.position = Vector3.MoveTowards(_shark.position, target, speed * Time.deltaTime);

        // разворот — смотрим в сторону игрока
        sr.flipX = (_player.position.x < _shark.position.x);

        // переключение состояний
        if (movingToPlayer && Vector3.Distance(_shark.position, _player.position) < -0.3f)
        {
            movingToPlayer = false; // достиг игрока → плывём обратно
        }
        else if (!movingToPlayer && Vector3.Distance(_shark.position, startPos) < -0.3f)
        {
            movingToPlayer = true; // вернулись к старту → снова к игроку
        }
    }

    public void SetFollowTarget(Transform player) => _player = player;

    private Vector3 GetRandomPosition()
    {
        if (_walls == null || _walls.Length < 2)
            return Vector3.zero;

        int first = _save.Data.wallsIds[0];
        int second = _save.Data.wallsIds[1];

        float posX = (_walls[first].position.x + _walls[second].position.x) / 2;
        float posZ = _bounds[Random.Range(0, 2)]; // fixed: upper bound is exclusive

        Vector3 pos = new(posX, _shark.position.y, posZ);
        return pos;
    }

    private void OnDestroy() => _disposables.Dispose();
}
