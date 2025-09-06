using System;
using System.Collections.Generic;

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

}
public enum EnvironmentType
{
    Sewerage,
    River,
    Ocean
}