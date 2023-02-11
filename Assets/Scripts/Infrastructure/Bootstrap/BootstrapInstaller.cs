using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public Bootstrapper _bootstrapper;

    public override void InstallBindings()
    {
        Container.Bind<IAssetsProvider>().FromInstance(new ResourcesAssetProvider()).AsSingle();
        ICoroutineRunner coroutineRunner = _bootstrapper;
        Container.Bind<ICoroutineRunner>().FromInstance(coroutineRunner).AsSingle();

        INetworkService networkService = new NetworkService(coroutineRunner);
        Container.Bind<INetworkService>().FromInstance(networkService).AsSingle();

        IStaticDataService staticDataService = RegisterStaticDataService();
        Container.Bind<IStaticDataService>().FromInstance(staticDataService).AsSingle();

        IUIFactory uIFactory = new UIFactory(staticDataService, coroutineRunner, networkService);
        Container.Bind<IUIFactory>().FromInstance(uIFactory).AsSingle();
        Container.Bind<IWindowService>().FromInstance(new WindowService(uIFactory)).AsSingle();

        //AllServices.Container.RegisterSingle<IAssetsProvider>(new ResourcesAssetProvider());
        //AllServices.Container.RegisterSingle<INetworkService>(new NetworkService(_coroutineRunner));
    }

    private IStaticDataService RegisterStaticDataService()
    {
        IStaticDataService staticDataService = new StaticDataService();
        staticDataService.Load();
        return staticDataService;
    }
}
