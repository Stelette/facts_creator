using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameStateMachine : IGameStateMachine
{
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public GameStateMachine(ICoroutineRunner coroutineRunner, DiContainer diContainer)
    {
        _states = new Dictionary<Type, IExitableState>()
        {
            [typeof(BootstrapState)] = new BootstrapState(this, coroutineRunner),
            [typeof(MainMenuState)] = new MainMenuState(this, diContainer.Resolve<IAssetsProvider>(), diContainer.Resolve<INetworkService>(), diContainer.Resolve<IWindowService>()),
            [typeof(GroupCreatorState)] = new GroupCreatorState(this, coroutineRunner, diContainer.Resolve<IAssetsProvider>(), diContainer.Resolve<INetworkService>()),
            [typeof(SubCategoryCreatorState)] = new SubCategoryCreatorState(this, coroutineRunner, diContainer.Resolve<IAssetsProvider>(), diContainer.Resolve<INetworkService>()),
            [typeof(CategoryCreatorState)] = new CategoryCreatorState(this, coroutineRunner, diContainer.Resolve<IAssetsProvider>(), diContainer.Resolve<INetworkService>())  
        };
    }


    public void Enter<TState>() where TState : class, IState
    {
        IState state = ChangeState<TState>();
        state.Enter();
    }

    public void Enter<TState, TPayLoad>(TPayLoad payload) where TState : class, IPayLoadedState<TPayLoad>
    {
        IPayLoadedState<TPayLoad> state = ChangeState<TState>();
        state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
        _activeState?.Exit();
        TState state = GetState<TState>();
        _activeState = state;
        return state;
    }


    private TState GetState<TState>() where TState : class, IExitableState
    {
        return _states[typeof(TState)] as TState;
    }
}
