using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<int> collectedTrashIds = new();
    public List<int> rescuedAnimalsIds = new();

    public EnvironmentType currentEnvironment;
    public int currentLevelIndex;
    public bool isFirstLaunch = true;

    public bool isTrashCollected;
    public bool isTrashSorted;
    public bool isAnimalRescued;

    public int currentScore;
    public int trashCount;
    public int animalsCount;

    public Vector3 playerPos;
}
public enum EnvironmentType
{
    Sewerage,
    River,
    Ocean
}