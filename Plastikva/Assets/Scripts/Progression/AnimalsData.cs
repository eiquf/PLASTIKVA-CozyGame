using UnityEngine;

[System.Serializable]
public struct AnimalsData
{
    [field: SerializeField] public int PersistentId { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Sprite ChangeIcon { get; private set; }
}