using UnityEngine;

public class TPlayerMoveState : IState<TestPlayer>
{
    public void EnterAction(TestPlayer instance)
    {
        Debug.Log("Enter MoveState");
    }

    public void UpdateAction(TestPlayer instance)
    {
        Debug.Log("Update MoveState");
    }

    public void ExitAction(TestPlayer instance)
    {
        Debug.Log("Exit MoveState");
    }
}
