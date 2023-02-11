using System;
using UnityEngine;

public class UIFactory : IUIFactory
{
    private readonly IStaticDataService _staticDataService;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly INetworkService _networkService;

    public UIFactory(IStaticDataService staticDataService,ICoroutineRunner coroutineRunner,INetworkService networkService)
    {
        _staticDataService = staticDataService;
        _coroutineRunner = coroutineRunner;
        _networkService = networkService;
    }

    public BaseWindow OpenWindow(WindowId windowId)
    {
        BaseWindow window = null;
        switch (windowId)
        {
            case WindowId.AddCategory:
                window = UnityEngine.Object.Instantiate(_staticDataService.ForWindow(windowId));
                break;
        }
        return window;
    }
}

