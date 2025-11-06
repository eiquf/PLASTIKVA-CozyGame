using UnityEngine;

public class TapTutor : Tutorial
{
    private bool _subscribed;

    public TapTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler)
        : base(tutorial, service, handler) { }

    public override void Handle(string request)
    {
        if (request == REQUEST_COMMAND)
        {
            if (!save.Data.isTapTaught)
            {
                tutor.SetActive(true);

                if (!_subscribed)
                {
                    _subscribed = true;
                    input.LeftMouseClicked += Collect;
                }
            }
            else
                tutor.SetActive(false);
        }
    }

    private void Collect()
    {
        save.Data.isTapTaught = true;
        tutor.SetActive(false);

        if (_subscribed)
        {
            input.LeftMouseClicked -= Collect;
            _subscribed = false;
        }
    }
}
