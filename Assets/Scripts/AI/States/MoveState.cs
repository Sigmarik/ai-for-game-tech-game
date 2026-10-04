using UnityEngine;

public class MoveState : AIState
{
    public MoveState(AIPlayerController ai, AIStateMachine stateMachine) : base(ai, stateMachine)
    {
    }

    public override void Enter()
    {
        AI.MoveTo(AI.moveTarget);
    }

    public override void Update()
    {
        if (AI.HasReachedDestination())
        {
            StateMachine.ChangeState(AI.IdleState);
        }
    }

    public override void Exit()
    {
        AI.StopMoving();
    }
}

