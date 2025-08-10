using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Trash/Level")]
public class TrashLevelDef : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }

    [field: SerializeField] public GameObject[] Animals { get; private set; }
    [field: SerializeField] public GameObject[] Trash { get; private set; }


    [field: SerializeField][Min(1)] public int RequiredTrashCount { get; private set; } = 5;
    [field: SerializeField][Min(1)] public int RequiredAnimalsCount { get; private set; } = 5;
}
