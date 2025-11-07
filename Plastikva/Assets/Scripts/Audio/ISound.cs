using R3;

public interface ISound
{
    public ReactiveCommand<int> PlayCommand { get; }

}
public enum GameSound
{
    Trash_PickUp = 0,
    Level_Unlock = 1,
    Shell = 2,
    Rescue = 3
    // Add
}
