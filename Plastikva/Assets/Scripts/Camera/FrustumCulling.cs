using System.Collections.Generic;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    [Header("Objects to Cull")]
    [SerializeField] private Transform[] _envPos; 
    private readonly List<GameObject> _objectsToCull = new();

    private IsometricCamera _camera;

    public void Initialize(IsometricCamera camera)
    {
        _camera = camera;

        _objectsToCull.Clear();

        foreach (Transform child in _envPos)
        {
            foreach (Transform child2 in child.GetComponentsInChildren<Transform>()) { 

            _objectsToCull.Add(child2.gameObject);
            }
        }
    }
    void FixedUpdate()
    {
        if (_camera == null || _camera.Camera == null)
            return;

        Camera cam = _camera.Camera;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (GameObject obj in _objectsToCull)
        {
            if (obj == null) continue;

            if (obj.TryGetComponent(out Renderer rend))
            {
                bool isVisible = GeometryUtility.TestPlanesAABB(planes, rend.bounds);
                obj.SetActive(isVisible);
            }
        }
    }
    private void AddChildrenRecursive(Transform parent)
    {
        _objectsToCull.Add(parent.gameObject);

        foreach (Transform child in parent)
            AddChildrenRecursive(child);
    }
}
