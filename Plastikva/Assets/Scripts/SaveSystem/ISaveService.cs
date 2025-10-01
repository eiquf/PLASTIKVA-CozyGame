public interface ISaveService
{
    GameData Data { get; }
    void LoadOrCreate();
    void Save();
    void Clear();
}
