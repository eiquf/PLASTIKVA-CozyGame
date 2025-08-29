using UnityEngine;

[CreateAssetMenu(menuName = "Trash/Level Set")]
public class TrashLevelSet : ScriptableObject
{
    [field: SerializeField] public TrashLevelDef[] Levels { get; private set; }
}
