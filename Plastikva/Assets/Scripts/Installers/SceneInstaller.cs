using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MonoBehaviour>()
        .WithId("CoroutineHost")
        .FromComponentInHierarchy()
        .AsSingle();

        BindInputs();
        BindViews();
    }
    private void BindInputs()
    {
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();

        Container.BindInterfacesAndSelfTo<PlayerInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<TrashInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<AnimalsInputHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<TutorInputHandler>().AsSingle();
    }
    private void BindViews()
    {
        Container.Bind<ScoreView>().AsSingle();
        Container.Bind<TrashSortView>().AsSingle();
    }
}
