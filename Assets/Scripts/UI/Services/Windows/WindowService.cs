using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WindowService : IWindowService
{
    private readonly IUIFactory _uIFactory;

    private BaseWindow _openWindow;

    public WindowService(IUIFactory uIFactory)
    {
        _uIFactory = uIFactory;
    }

    public void Open(WindowId windowId)
    {
        _openWindow = _uIFactory.OpenWindow(windowId);
        //_openWindow = UnityEngine.Object.Instantiate(_staticDataService.ForWindow(windowId).gameObject);
    }
}