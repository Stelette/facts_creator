public interface IGameStateMachine
{
    void Enter<TState>() where TState : class, IState;
    void Enter<TState, TPayLoad>(TPayLoad payload) where TState : class, IPayLoadedState<TPayLoad>;
}