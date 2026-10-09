using Unity.VisualScripting;
using UnityEngine;

public class InteractState : AIState
{
    public float duration = 2f;
    private float timer = 0f;
    public InteractState(AIPlayerController ai, AIStateMachine stateMachine) : base(ai, stateMachine)
    {
        
    }

    public override void Enter()
    {
        timer = duration;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
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


