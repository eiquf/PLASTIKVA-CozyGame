[System.Serializable]
public class GameData
{
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