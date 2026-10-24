using Unity.VisualScripting;
using UnityEngine;

public class InteractState : AIState
{
    public float duration = 2f;
    public InteractState(AIPlayerController ai, AIStateMachine stateMachine) : base(ai, stateMachine)
    {
        
    }

    public override void Enter()
    {
        if (AI.controlsFrozen) return;
        if (Vector3.Distance(AI.transform.position, AI.Brain.currentTask.interactable.transform.position) > 2f)
        {
            StateMachine.ChangeState(AI.MoveState);
            return;
        }
        AI.Brain.StartInteraction();
    }

    public override void Update()
    {
        if (AI.Brain.IsCurrentTaskClaimedByPlayer())
        {
            AI.Brain.AbandonCurrentTask();
            return;
        }

        if (AI.Brain.IsStepDone())
        {
            StateMachine.ChangeState(AI.IdleState);
            AI.Brain.OnInteractComplete();
        }
    }

    public override void Exit()
    {
        // AI.StopMoving();
    }
}


