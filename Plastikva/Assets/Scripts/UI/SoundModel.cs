public class SoundModel
{
    private ISaveService _save;
    private int _currentSound;

    public void SetUp(ISaveService save)
    {
        _save = save;
        _currentSound = _save.Data.sound;
    }
    public int ChangeVolume(bool state)
    {
        _currentSound = state ? 1 : 0;

        _save.Data.sound = _currentSound;

        return _currentSound;
    }
}
