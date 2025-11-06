using UnityEngine;

public class TrashInstance : MonoBehaviour
{
    public SpriteRenderer sprite { get; private set; }
    public int Id;
    private void Start() => sprite = GetComponent<SpriteRenderer>();
}
