using UnityEngine;

public class LevelGeneratorModel
{
    private GameObject _trashPref;
    private GameObject _animalPref;
    private Transform _plane;
    private TrashData[] _trashData = System.Array.Empty<TrashData>();
    private AnimalsData[] _animalsData = System.Array.Empty<AnimalsData>();

    public void Setup(TrashData[] trashData, AnimalsData[] animalsData, Transform plane, GameObject trashPref, GameObject animalPref)
    {
        _trashData = trashData ?? System.Array.Empty<TrashData>();
        _animalsData = animalsData ?? System.Array.Empty<AnimalsData>();

        _plane = plane;
        _trashPref = trashPref;
        _animalPref = animalPref;
    }

    public void Generate()
    {
        float planeXSize = _plane.localScale.x * 10f;
        float planeZSize = _plane.localScale.z * 10f;
        Vector3 center = _plane.position;

        for (int i = 0; i < _trashData.Length; i++)
        {
            var td = _trashData[i];

            float x = Random.Range(center.x - planeXSize / 2f, center.x + planeXSize / 2f);
            float z = Random.Range(center.z - planeZSize / 2f, center.z + planeZSize / 2f);
            Vector3 pos = new(x, center.y, z);

            var trash = Object.Instantiate(_trashPref, pos, Quaternion.identity);

            SpriteRenderer render = trash.GetComponent<SpriteRenderer>();
            render.sprite = td.Icon;
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
