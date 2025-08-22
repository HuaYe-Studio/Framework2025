using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public StateMachine<TestPlayer> StateMachine { get; private set; }
    
    public TPlayerIdleState IdleState { get; private set; }
    public TPlayerMoveState MoveState { get; private set; }
    public TPlayerJumpState JumpState { get; private set; }
    

    void Start()
    {
        
        StateMachine = new StateMachine<TestPlayer>(this);
        
        IdleState = new TPlayerIdleState();
        MoveState = new TPlayerMoveState();
        JumpState = new TPlayerJumpState();
        
        StateMachine.Init(IdleState);
    }

    void Update()
    {
        StateMachine.UpdateState();
        
        if (Input.GetKeyDown(KeyCode.Space))
            StateMachine.ChangeState(JumpState);
        if (Input.GetKeyDown(KeyCode.M))
            StateMachine.ChangeState(MoveState);
        if (Input.GetKeyDown(KeyCode.I))
            StateMachine.ChangeState(IdleState);
            
    }
}
