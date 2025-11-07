public class SoundModel
{
    private ISaveService _save;
    private int _currentSound; 

    public void SetUp(ISaveService save)
    {
        _save = save;
        _currentSound = _save.Data.sound;
    }

    public float ChangeVolume(bool isMuted)
    {
        _save.Data.sound = isMuted ? 0 : 1;
        _save.Save();
        return isMuted ? 0f : 1f;
    }

    public float GetVolume() => _currentSound == 1 ? 1f : 0f;

    public bool IsMuted => _currentSound == 0;
}
