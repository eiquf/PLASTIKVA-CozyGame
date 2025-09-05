using UnityEngine;

public class LevelGeneratorModel
{
    private GameObject _trashPref;
    private GameObject _animalPref;
    private Transform _plane;
    private TrashData[] _trashData = System.Array.Empty<TrashData>();
    private AnimalsData[] _animalsData = System.Array.Empty<AnimalsData>();
    private ISaveService _save;
    public void Setup(GameObject trashPref, GameObject animalPref, ISaveService save)
    {
        _trashPref = trashPref;
        _animalPref = animalPref;
        _save = save;
    }
    public void SetupData(TrashData[] trashData, AnimalsData[] animalsData, Transform plane)
    {
        _trashData = trashData ?? System.Array.Empty<TrashData>();
        _animalsData = animalsData ?? System.Array.Empty<AnimalsData>();
        _plane = plane;
    }
    public void Generate()
    {
        float planeXSize = _plane.localScale.x * 50f;
        float planeZSize = _plane.localScale.z * 10f;
        Vector3 center = _plane.position;

        var collected = _save.Data.collectedTrashIds;
        
        for (int i = 0; i < _trashData.Length; i++)
        {
            if (collected != null && collected.Contains(i))
                continue;

            var td = _trashData[i];

            float x = Random.Range(center.x - planeXSize / 2f, center.x + planeXSize / 2f);
            float z = Random.Range(center.z - planeZSize / 2f, center.z + planeZSize / 2f);
            Vector3 pos = new(x, center.y * -1f, z);

            var go = Object.Instantiate(_trashPref, pos, Quaternion.identity);

            if (go.TryGetComponent<SpriteRenderer>(out var render))
                render.sprite = td.Icon;

            var inst = go.GetComponent<TrashInstance>() ?? go.AddComponent<TrashInstance>();
            inst.Id = i;
        }

        for (int i = 0; i < _animalsData.Length; i++)
        {
            var td = _animalsData[i];

            float x = Random.Range(center.x - planeXSize / 2f, center.x + planeXSize / 2f);
            float z = Random.Range(center.z - planeZSize / 2f, center.z + planeZSize / 2f);
            Vector3 pos = new(x, center.y, z);

            var trash = Object.Instantiate(_animalPref, pos, Quaternion.identity);

            SpriteRenderer render = trash.GetComponent<SpriteRenderer>();
            render.sprite = td.Icon;
        }
    }
}
