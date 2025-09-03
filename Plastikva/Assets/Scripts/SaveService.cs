public class SaveService : ISaveService
{
    private GameData _data;

    public GameData Data => _data;

    public void LoadOrCreate() => _data = SaveLoadLevel.Load<GameData>() ?? new GameData();

    public void Save() => SaveLoadLevel.Save(_data);
}
