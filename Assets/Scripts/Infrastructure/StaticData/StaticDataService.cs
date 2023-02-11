using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticDataService : IStaticDataService
{
    private Dictionary<WindowId, BaseWindow> _windows;

    public void Load()
    {
        _windows = Resources.Load<WindowConfigWrapper>(AssetPath.StaticDataWindow).windowConfigs
            .ToDictionary(x => x.WindowId, x => x.Window);
    }

    public BaseWindow ForWindow(WindowId windowId) =>
        _windows.TryGetValue(windowId, out BaseWindow window)
        ? window
        : null;
}
