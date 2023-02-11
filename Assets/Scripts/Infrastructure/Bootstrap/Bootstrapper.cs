using UnityEngine;
using System.Collections;
using Zenject;

public class Bootstrapper: MonoBehaviour, ICoroutineRunner
{
    private DiContainer _diContainer;

    [Inject]
    public void Binding(DiContainer container)
    {
        _diContainer = container;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        GameStateMachine stateMachine = new GameStateMachine(this, _diContainer);
        stateMachine.Enter<BootstrapState>();
    }
}
