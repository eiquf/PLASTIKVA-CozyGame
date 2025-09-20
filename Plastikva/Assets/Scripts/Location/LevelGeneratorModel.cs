using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorModel
{
    private GameObject _trashPref;
    private GameObject _animalPref;

    private Transform _currentPlane;
    private readonly List<Vector2> _points = new();

    float minDistance = -2f;
    float maxDistance = 2f;

    private TrashData[] _trashData;
    private AnimalsData[] _animalsData;
    private ISaveService _save;
    public void Setup(GameObject trashPref, GameObject animalPref, ISaveService save)
    {
        _trashPref = trashPref;
        _animalPref = animalPref;
        _save = save;
    }
    public void SetupData(TrashData[] trashData, AnimalsData[] animalsData, Transform plane)
    {
        _trashData = trashData;
        _animalsData = animalsData;
        _currentPlane = plane;

        _currentPlane.gameObject.SetActive(true);
    }
    public void Generate()
    {
        float planeXSize = _currentPlane.localScale.x * 50f;
        float planeZSize = _currentPlane.localScale.z * 37f;
        Vector3 center = _currentPlane.position;

        var collected = _save.Data.collectedTrashIds;

        for (int i = 0; i < _trashData.Length; i++)
        {
            if (collected != null && collected.Contains(_trashData[i].PersistentId))
                continue;

            var td = _trashData[i];

            Vector3 pos = GeneratePoint(center, planeXSize, planeZSize);

            var go = Object.Instantiate(_trashPref, pos, Quaternion.identity);

            if (go.TryGetComponent<SpriteRenderer>(out var render))
                render.sprite = td.Icon;

            var inst = go.GetComponent<TrashInstance>() ?? go.AddComponent<TrashInstance>();
            inst.Id = td.PersistentId;
        }

        var rescued = _save.Data.rescuedAnimalsIds;

        for (int i = 0; i < _animalsData.Length; i++)
        {
            var ad = _animalsData[i];

            Vector3 pos = GeneratePoint(center, planeXSize, planeZSize);

            var go = Object.Instantiate(_animalPref, pos, Quaternion.identity);

            var inst = go.GetComponent<AnimalInstance>() ?? go.AddComponent<AnimalInstance>();
            inst.Id = ad.PersistentId;

            var wasRescued = rescued != null && rescued.Contains(ad.PersistentId);
            var spriteToUse = wasRescued && ad.ChangeIcon != null ? ad.ChangeIcon : ad.Icon;

            if (inst.Render != null)
                inst.Render.sprite = spriteToUse;
        }
    }
    Vector3 GeneratePoint(Vector3 center, float planeXSize, float planeZSize)
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            float x = Random.Range(center.x - planeXSize / 2f, center.x + planeXSize / 2f);
            float z = Random.Range(center.z - planeZSize / 2f, center.z + planeZSize / 2f);
            Vector3 candidate = new(x, center.y * -0.05f, z);

            bool valid = true;
            foreach (var p in _points)
            {
                float dx = candidate.x - p.x;
                float dz = candidate.y - p.y;
                if (dx * dx + dz * dz < minDistance * minDistance)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                _points.Add(candidate);
                return candidate;
            }
        }

        return Vector3.zero;
    }
}
