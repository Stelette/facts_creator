using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootstrapState : IState
{
    private readonly GameStateMachine _stateMachine;
    private readonly ICoroutineRunner _coroutineRunner;

    public BootstrapState(GameStateMachine stateMachine,ICoroutineRunner coroutineRunner)
    {
        _stateMachine = stateMachine;
        _coroutineRunner = coroutineRunner;
        RegisterServices();
    }

    public void Enter()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    public void Exit()
    {
    }


    private void RegisterServices()
    {
        //AllServices.Container.RegisterSingle<IAssetsProvider>(new ResourcesAssetProvider());
        //AllServices.Container.RegisterSingle<INetworkService>(new NetworkService(_coroutineRunner));
    }
}
