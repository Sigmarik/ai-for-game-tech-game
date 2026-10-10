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
        AI.Brain.StartInteraction();
    }

    public override void Update()
    {
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


