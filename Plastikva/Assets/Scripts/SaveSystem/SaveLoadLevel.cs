using System;
using System.IO;
using UnityEngine;

public class SaveLoadLevel
{
    [Serializable] private class Box<T> { public T value; }

    private static string GetPath() =>
       Path.Combine(Application.persistentDataPath, "SaveData.json");
    public static void Save<T>(T data)
    {
        var box = new Box<T> { value = data };
        string json = JsonUtility.ToJson(box, prettyPrint: true);
        File.WriteAllText(GetPath(), json);
    }

    public static T Load<T>(T defaultValue = default)
    {
        string path = GetPath();
        if (!File.Exists(path)) return defaultValue;

        string json = File.ReadAllText(path);
        var box = JsonUtility.FromJson<Box<T>>(json);
        return box != null ? box.value : defaultValue;
    }

    public static void ClearSaveData()
    {
        string filePath = GetPath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save data cleared!");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }
}