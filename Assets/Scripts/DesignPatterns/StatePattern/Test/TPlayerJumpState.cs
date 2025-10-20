using UnityEngine;

public class TPlayerJumpState : IState<TestPlayer>
{
    public void EnterAction(TestPlayer instance)
    {
        Debug.Log("Enter JumpState");
    }

    public void UpdateAction(TestPlayer instance)
    {
        Debug.Log("Update JumpState");
    }

    public void ExitAction(TestPlayer instance)
    {
        Debug.Log("Exit JumpState");
    }
}
