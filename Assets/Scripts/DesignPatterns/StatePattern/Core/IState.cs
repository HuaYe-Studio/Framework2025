using UnityEngine;

public interface IState<T> 
{
    public void EnterAction(T instance);
    public void UpdateAction(T instance);
    public void ExitAction(T instance);
}
