using UnityEngine;

public class TPlayerIdleState : IState<TestPlayer>
{
    public void EnterAction(TestPlayer instance)
    {
        Debug.Log("Enter IdleState");
    }

    public void UpdateAction(TestPlayer instance)
    {
        Debug.Log("Update IdleState");
    }

    public void ExitAction(TestPlayer instance)
    {
        Debug.Log("Exit IdleState");
    }
}
