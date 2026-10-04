public class IdleState : AIState
{
    public IdleState(AIPlayerController ai, AIStateMachine stateMachine) : base(ai, stateMachine)
    {
        
    }

    public override void Enter()
    {
        // Stop moving
        AI.StopMoving();
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
    }
}
