using UnityEngine;

public class MoveTutor : Tutorial
{
    public MoveTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler) : base(tutorial, service, handler) { }
    public override void Handle(string request)
    {
        if (request == REQUEST_COMMAND)
        {
            if (save.Data.isMovementTaught == false)
            {
                tutor.SetActive(true);
                if (input.MovementInput().sqrMagnitude > 0.01f)
                {
                    save.Data.isMovementTaught = true;
                    tutor.SetActive(false);
                }
            }
        }

    }
}
