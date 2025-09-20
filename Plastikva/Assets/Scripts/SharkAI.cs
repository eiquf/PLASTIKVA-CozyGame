using UnityEngine;

public class SharkAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float zigzagAmplitude = 0.2f;
    public float zigzagFrequency = 5f;
    public float desyncDelay = 0.5f;

    private Vector3 startPos;
    private bool movingToPlayer = true;
    private SpriteRenderer sr;
    private float offsetTime;
    private int startFlip;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        startFlip = sr.flipX ? -1 : 1;
        offsetTime = Random.Range(0f, desyncDelay); // акулы плавают не синхронно
    }

    void Update()
    {
        if (player == null) return;

        // маленький зигзаг по синусу
        float zigzag = Mathf.Sin((Time.time + offsetTime) * zigzagFrequency) * zigzagAmplitude;

        Vector3 target;
        if (movingToPlayer)
        {
            target = new Vector3(player.position.x, player.position.y + zigzag, transform.position.z);
        }
        else
        {
            target = new Vector3(startPos.x, startPos.y + zigzag, transform.position.z);
        }

        // движение к цели
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // проверка разворота игрока (flip)
        if (player.TryGetComponent(out SpriteRenderer playerSR))
        {
            if (playerSR.flipX)
                sr.flipX = false;
            else
                sr.flipX = true;
        }

        // переключение состояний
        if (movingToPlayer && Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingToPlayer = false; // достиг игрока → назад
        }
        else if (!movingToPlayer && Vector3.Distance(transform.position, startPos) < 0.1f)
        {
            movingToPlayer = true; // достиг старта → снова к игроку
        }
    }
}
