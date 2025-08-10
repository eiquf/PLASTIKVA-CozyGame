[System.Serializable]
public class LevelData
{
    public enum EnvironmentType
    {
        Sewerage,
        River,
        Ocean
    }

    public EnvironmentType currentEnvironment;
    public int currentLevelIndex;
    public bool isFirstLaunch = true;

    public bool isTrashCollected;
    public bool isAnimalRescued;
}