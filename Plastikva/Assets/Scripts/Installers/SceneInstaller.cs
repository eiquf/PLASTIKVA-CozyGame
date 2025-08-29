using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindInputs();
        BindViews();
    }
    private void BindInputs()
    {
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();

        Container.BindInterfacesAndSelfTo<PlayerInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<TrashInputHandler>().AsSingle();
    }
    private void BindViews()
    {
        Container.Bind<ScoreView>().AsSingle();
        Container.Bind<TrashSortView>().AsSingle();
    }
}
