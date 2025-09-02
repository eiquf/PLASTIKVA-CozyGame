using UnityEngine;

[CreateAssetMenu(menuName = "Trash/Level")]
public class TrashLevelDef : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }

    [field: SerializeField] public GameObject[] AnimalsPref { get; private set; }
    [field: SerializeField] public GameObject[] TrashPref { get; private set; }

    [field: SerializeField] public TrashData[] Trash { get; private set; }
    [field: SerializeField][Min(1)] public int RequiredTrashCount { get; private set; } = 5;
    [field: SerializeField][Min(1)] public int RequiredAnimalsCount { get; private set; } = 5;
}
