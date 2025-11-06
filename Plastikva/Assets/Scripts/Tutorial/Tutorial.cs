using UnityEngine;

public abstract class Tutorial
{
    protected GameObject tutor;
    protected ISaveService save;
    protected TutorInputHandler input;

    public Tutorial NextHandler { get; private set; }

    protected const string REQUEST_COMMAND = "Next";

    public Tutorial(GameObject tutorial, ISaveService service, TutorInputHandler handler)
    {
        tutor = tutorial;
        save = service;
        input = handler;
    }

    public Tutorial SetNext(Tutorial handler)
    {
        NextHandler = handler;
        return handler;
    }

    public virtual void Handle(string request) => NextHandler?.Handle(request);
}
