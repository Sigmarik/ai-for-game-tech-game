public class AIStateMachine
{
    public AIState CurrentState { get; private set; }

    public void Initialize(AIState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(AIState newState)
    {
        if (newState == null)
            return;

        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}