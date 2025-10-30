using System.Collections.Generic;
using UnityEngine;

public class MapView
{
    private RectTransform _fishPos;
    private List<GameObject> _points = new();

    public void SetUp(UI ui)
    {
        _fishPos = ui.FishPos;
        _points = ui.Points;
    }
    public void UpdateView(int index)
    {
        for (int i = 0; i < index && i < _points.Count; i++)
        {
            if (_points[i] != null)
                _points[i].SetActive(false);
        }

        _fishPos.localPosition = _points[index].transform.localPosition;
    }
}