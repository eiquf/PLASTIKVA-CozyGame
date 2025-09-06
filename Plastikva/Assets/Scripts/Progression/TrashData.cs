using UnityEngine;

[System.Serializable]
public struct TrashData
{
    [field: SerializeField] public int PersistentId {  get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsRecyclable { get; private set; }
}
