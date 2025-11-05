using UnityEngine;

public class AnimalInstance : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer Render { get; private set; }
    [field: SerializeField] public GameObject Bubbles { get; private set; }
    [field: SerializeField] public GameObject Arrow { get; private set; }
    public int Id;
    public bool IsStatic;
}