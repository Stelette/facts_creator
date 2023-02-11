public class SubCategoryCreatorState : IPayLoadedState<string>
{
    private readonly IGameStateMachine _stateMachine;
    private readonly IAssetsProvider _assetsProvider;
    private readonly INetworkService _networkService;
    private readonly ICoroutineRunner _coroutineRunner;

    private SubCategoryCreator _subCategoryCreator;

    public SubCategoryCreatorState(IGameStateMachine stateMachine, ICoroutineRunner coroutineRunner, IAssetsProvider assetsProvider, INetworkService networkService)
    {
        _stateMachine = stateMachine;
        _assetsProvider = assetsProvider;
        _networkService = networkService;
        _coroutineRunner = coroutineRunner;
    }

    public void Enter(string category)
    {
        _subCategoryCreator = _assetsProvider.Instantiate(AssetPath.SubCategoryCreator).GetComponent<SubCategoryCreator>();

        _subCategoryCreator.Construct(
            _stateMachine,
            _coroutineRunner,
            _networkService);

        _subCategoryCreator.Init(category);
    }

    public void Exit()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (_subCategoryCreator != null)
            UnityEngine.Object.Destroy(_subCategoryCreator.gameObject);
    }
}