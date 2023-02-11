using System;
using UnityEngine;

public class GroupCreatorState : IPayLoadedState<CategoryInfoWrapper>
{
    private readonly IGameStateMachine _stateMachine;
    private readonly IAssetsProvider _assetsProvider;
    private readonly INetworkService _networkService;
    private readonly ICoroutineRunner _coroutineRunner;

    private GroupCreator _groupCreator;

    public GroupCreatorState(IGameStateMachine stateMachine, ICoroutineRunner coroutineRunner, IAssetsProvider assetsProvider, INetworkService networkService)
    {
        _stateMachine = stateMachine;
        _assetsProvider = assetsProvider;
        _networkService = networkService;
        _coroutineRunner = coroutineRunner;
    }

    public void Enter(CategoryInfoWrapper categoryWrapper)
    {
        _groupCreator = _assetsProvider.Instantiate(AssetPath.GroupCreator).GetComponent<GroupCreator>();

        _groupCreator.Construct(
            _stateMachine,
            _coroutineRunner,
            _groupCreator.GetComponentInChildren<IImageProvider>(),
            _networkService,
            categoryWrapper);

        _groupCreator.Init();
    }

    public void Exit()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (_groupCreator != null)
            UnityEngine.Object.Destroy(_groupCreator.gameObject);
    }
}