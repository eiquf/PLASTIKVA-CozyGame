using UnityEngine;

[System.Serializable]
public struct TrashData
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject Pref { get; private set; }
    [field: SerializeField] public bool IsRecyclable { get; private set; }
}