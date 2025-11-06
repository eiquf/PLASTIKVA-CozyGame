using UnityEngine;

public class EnemyTutor : Tutorial
{
    private bool _subscribed;

    public EnemyTutor(GameObject tutorial, ISaveService service, TutorInputHandler handler)
        : base(tutorial, service, handler) { }

    public override void Handle(string request)
    {
        if (request == REQUEST_COMMAND)
        {
            if (!save.Data.isEnemyTaught)
            {
                tutor.SetActive(true);

                if (!_subscribed)
                {
                    _subscribed = true;
                    input.CameraRotated += Complete;
                }
            }
            else
                tutor.SetActive(false);
        }
    }

    private void Complete()
    {
        save.Data.isEnemyTaught = true;
        tutor.SetActive(false);

        input.CameraRotated -= Complete;
        _subscribed = false;
    }
}
