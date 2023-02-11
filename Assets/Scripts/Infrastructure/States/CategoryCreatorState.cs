public class CategoryCreatorState : IState
{
    private readonly IGameStateMachine _stateMachine;
    private readonly IAssetsProvider _assetsProvider;
    private readonly INetworkService _networkService;
    private readonly ICoroutineRunner _coroutineRunner;

    private CategoryCreator _categoryCreator;

    public CategoryCreatorState(IGameStateMachine stateMachine, ICoroutineRunner coroutineRunner, IAssetsProvider assetsProvider, INetworkService networkService)
    {
        _stateMachine = stateMachine;
        _assetsProvider = assetsProvider;
        _networkService = networkService;
        _coroutineRunner = coroutineRunner;
    }

    public void Enter()
    {
        _categoryCreator = _assetsProvider.Instantiate(AssetPath.CategoryCreator).GetComponent<CategoryCreator>();

        _categoryCreator.Construct(
            _stateMachine,
            _coroutineRunner,
            _networkService,
            _categoryCreator.GetComponentInChildren<IImageProvider>(),
            _categoryCreator.GetComponentInChildren<ICategoryTypeProvider>());
    }

    public void Exit()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (_categoryCreator != null)
            UnityEngine.Object.Destroy(_categoryCreator.gameObject);
    }
}