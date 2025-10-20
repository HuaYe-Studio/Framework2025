using Unity.VisualScripting;
using UnityEngine;

public class StateMachine<T>
{
    public IState<T> CurrentState {get; private set;}

    private T _owner;

    public StateMachine(T instance)
    {
        _owner = instance;
    }

    public void Init(IState<T> startState)
    {
        CurrentState = startState;
        CurrentState?.EnterAction(_owner);
    }

    public void ChangeState(IState<T> newState)
    {
        CurrentState?.ExitAction(_owner);
        CurrentState = newState;
        CurrentState?.EnterAction(_owner);
    }
    
    public void UpdateState()
    {
        CurrentState?.UpdateAction(_owner);
    }
}
