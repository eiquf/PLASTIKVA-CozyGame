using UnityEngine;

public class AnimalInstance : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer Render { get; private set; }
    public int Id;
}