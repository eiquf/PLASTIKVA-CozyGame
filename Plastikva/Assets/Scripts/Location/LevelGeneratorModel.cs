using UnityEngine;

public class LevelGeneratorModel
{
    private Transform _plane;
    private TrashData[] _data = System.Array.Empty<TrashData>();

    public void Setup(TrashData[] data, Transform plane)
    {
        _plane = plane;
        _data = data ?? System.Array.Empty<TrashData>();
    }

    public void Generate()
    {
        float planeXSize = _plane.localScale.x * 10f;
        float planeZSize = _plane.localScale.z * 10f;
        Vector3 center = _plane.position;

        for (int i = 0; i < _data.Length; i++)
        {
            var td = _data[i];

            float x = Random.Range(center.x - planeXSize / 2f, center.x + planeXSize / 2f);
            float z = Random.Range(center.z - planeZSize / 2f, center.z + planeZSize / 2f);
            Vector3 pos = new(x, center.y, z);

            Object.Instantiate(td.Pref, pos, Quaternion.identity);
        }
    }
}
