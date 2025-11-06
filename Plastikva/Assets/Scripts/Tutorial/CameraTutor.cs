using UnityEngine;

public class CameraTutor : Tutorial
{
    private bool _subscribed;

    public CameraTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler)
        : base(tutorial, service, handler) { }

    public override void Handle(string request)
    {
        if (request == REQUEST_COMMAND)
        {
            if (!save.Data.isCameraTaught)
            {
                tutor.SetActive(true);

                if (!_subscribed)
                {
                    input.CameraDragged += Complete;
                    input.CameraZoomed += Complete;
                    _subscribed = true;
                }
            }
        }
    }

    private void Complete()
    {
        save.Data.isCameraTaught = true;
        tutor.SetActive(false);

        input.CameraDragged -= Complete;
        input.CameraZoomed -= Complete;
        _subscribed = false;
    }
}
