using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class OpenWindowButton : MonoBehaviour
{
    public Button Button;
    public WindowId WindowId;

    private IWindowService _windowService;

    public void Construct(IWindowService windowService)
    {
        _windowService = windowService;
    }

    private void Awake()
    {
        Button.onClick.AddListener(Open);
    }

    private void Open()
    {
        _windowService.Open(WindowId);
    }

    private void OnDestroy()
    {
        Button.onClick.RemoveListener(Open);
    }
}
