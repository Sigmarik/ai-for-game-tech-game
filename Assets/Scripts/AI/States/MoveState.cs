using UnityEngine;

public class MoveState : AIState
{
    public MoveState(AIPlayerController ai, AIStateMachine stateMachine) : base(ai, stateMachine)
    {
    }

    public override void Enter()
    {
        AI.MoveTo(AI.Brain.currentTask.interactable.transform.position);
    }

    public override void Update()
    {
        if (AI.HasReachedDestination())
        {
            AI.Brain.OnMoveComplete();
        }
    }

    public override void Exit()
    {
        AI.StopMoving();
    }
}

