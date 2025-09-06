using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveService : ISaveService
{
    private GameData _data;

    public GameData Data => _data;

    public void Clear() => SaveLoadLevel.ClearSaveData();

    public void LoadOrCreate()
    {
        _data = SaveLoadLevel.Load<GameData>() ?? new GameData();
        Debug.Log(_data.collectedTrashIds.Count);

    }

    public void Save() => SaveLoadLevel.Save(_data);
}
