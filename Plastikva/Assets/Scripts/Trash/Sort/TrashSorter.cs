using UnityEngine;
using Zenject;

public class TrashSorter : MonoBehaviour
{
    [Inject] private TrashInputHandler _input;

    private TrashLevelDef _currentLevel;

    private readonly LayerMask TrashMask = 1 << 6;
    private readonly LayerMask BioMask = 1 << 6;
    private IHitDetector _hitDetector;

}
