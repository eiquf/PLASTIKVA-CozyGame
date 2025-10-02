using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : ICamera<List<GameObject>>
{
    private readonly ICameraContext _context;

    private readonly List<GameObject> _objectsToCull = new();

    private Plane[] _planes;
    public CameraCulling(ICameraContext context)
    {
        _context = context;
    }
    public void Execute(Transform transform, List<GameObject> objects)
    {
        if (_context.Camera == null) return;
        _planes = GeometryUtility.CalculateFrustumPlanes(_context.Camera);

        foreach (GameObject obj in _objectsToCull)
        {
            if (obj == null) continue;

            if (!obj.TryGetComponent<Renderer>(out var rend)) continue;

            bool isVisible = GeometryUtility.TestPlanesAABB(_planes, rend.bounds);

            if (isVisible && !obj.activeSelf)
                obj.SetActive(true);
            else if (!isVisible && obj.activeSelf)
                obj.SetActive(false);
        }
    }
}