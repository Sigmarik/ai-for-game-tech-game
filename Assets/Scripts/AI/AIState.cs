public abstract class AIState
{
    protected AIPlayerController AI;
    protected AIStateMachine StateMachine;

    protected AIState(AIPlayerController ai, AIStateMachine stateMachine)
    {
        AI = ai;
        StateMachine = stateMachine;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}