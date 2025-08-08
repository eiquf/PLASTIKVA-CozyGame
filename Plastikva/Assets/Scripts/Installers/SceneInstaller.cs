using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindInputs();
        BindUI();
    }
    private void BindInputs()
    {
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();

        Container.BindInterfacesAndSelfTo<PlayerInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<TrashInputHandler>().AsSingle();
    }
    private void BindUI()
    {
        Container.Bind<TrashView>().AsSingle();
    }
}
