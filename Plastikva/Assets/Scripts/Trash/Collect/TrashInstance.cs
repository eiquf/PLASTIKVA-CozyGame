using UnityEngine;

public class TrashInstance : MonoBehaviour
{
    public SpriteRenderer sprite { get; private set; }
    public int Id;
    [field:SerializeField] public GameObject Arrow { get; private set; }
    private void Start() => sprite = GetComponent<SpriteRenderer>();
}
