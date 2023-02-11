using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : IState
{
    private readonly IGameStateMachine _stateMachine;
    private readonly IAssetsProvider _assetsProvider;
    private readonly INetworkService _networkService;
    private readonly IWindowService _windowService;

    private MainMenu _mainMenu;

    public MainMenuState(IGameStateMachine stateMachine, IAssetsProvider assetsProvider,INetworkService networkService, IWindowService windowService)
    {
        _stateMachine = stateMachine;
        _assetsProvider = assetsProvider;
        _networkService = networkService;
        _windowService = windowService;
    }

    public void Enter()
    {
        if(_mainMenu == null)
        {
            _mainMenu = _assetsProvider.Instantiate(AssetPath.MainMenu).GetComponent<MainMenu>();
            ICategoryTypeProvider provider = _mainMenu.GetComponentInChildren<ICategoryTypeProvider>();
            _mainMenu.Construct(_stateMachine,
                new CategoryProvider(_networkService),
                new GroupProvider(_networkService),
                provider);

            _mainMenu.Init();
            foreach (OpenWindowButton openWindowButton in _mainMenu.GetComponentsInChildren<OpenWindowButton>())
            {
                openWindowButton.Construct(_windowService);
            }
        }   
    }

    public void Exit()
    {
        //Debug.Log("At this stage there is nowhere to get out of this state");
    }
}