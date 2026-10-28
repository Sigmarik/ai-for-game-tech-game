using UnityEngine;
using UnityEngine.AI;

public enum AITaskSchedulingMode
{
    Closest,
    Furthest
}

public class AIPlayerController : GenericController
{
    [SerializeField] private float moveSpeed = 3f;

    public Vector3 moveTarget { get; private set; }

    public Animator animator;
    private static readonly int speedHash = Animator.StringToHash("MoveSpeed");
    public AIStateMachine StateMachine { get; private set; }

    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public InteractState InteractState { get; private set; }
    public AIBrain Brain { get; private set; }

    public AITaskSchedulingMode TaskSchedulingMode = AITaskSchedulingMode.Closest;

    private NavMeshAgent agent;

    [SerializeField] private string currentStateName;
    private string previousStateName;

    [SerializeField] public Transform testTarget;

    private float m_idleCheckTimer = 0f;
    private const float IdleCheckInterval = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }

        CharacterMovement characterMovement = GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.enabled = false;
        }

        StateMachine = new AIStateMachine();

        IdleState = new IdleState(this, StateMachine);
        MoveState = new MoveState(this, StateMachine);
        InteractState = new InteractState(this, StateMachine);

        Brain = new AIBrain(this);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);

        m_interact = GetComponent<Interact>();

        ExperimentLogger.LogEvent("AI", "controller initialized",
            "name", name,
            "mode", TaskSchedulingMode.ToString(),
            "position", transform.position.ToString("F2"));
    }

    private void Update()
    {
        if (controlsFrozen)
        {
            StopMoving();
            UpdateAnimator();
            return;
        }

        StateMachine.Update();

        UpdateAnimator();

        currentStateName = StateMachine.CurrentState?.GetType().Name;

        if (currentStateName != previousStateName)
        {
            ExperimentLogger.LogEvent("AI_STATE", "state changed",
                "ai", name,
                "from", previousStateName ?? "none",
                "to", currentStateName ?? "none",
                "mode", TaskSchedulingMode.ToString());
            previousStateName = currentStateName;
        }

        if (StateMachine.CurrentState == InteractState && Brain.currentTask != null)
        {
            if (Brain.currentTask.interactable.Interacter != m_interact && !Brain.currentTask.IsComplete)
            {
                ExperimentLogger.LogEvent("AI_ABORT", "interaction taken by another",
                    "ai", name,
                    "task", Brain.currentTask.name);
                Brain.AbandonCurrentTask();
            }
        }
        else if (Brain.currentTask != null && !Brain.currentTask.IsComplete && !Brain.IsTaskValid(Brain.currentTask))
        {
            ExperimentLogger.LogEvent("AI_ABORT", "current task no longer valid",
                "ai", name,
                "task", Brain.currentTask.name);
            Brain.AbandonCurrentTask();
        }
        else if (StateMachine.CurrentState == IdleState)
        {
            m_idleCheckTimer += Time.deltaTime;
            if (m_idleCheckTimer >= IdleCheckInterval)
            {
                m_idleCheckTimer = 0f;
                Brain.RequestWork();
            }
        }
        else
        {
            m_idleCheckTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    public void StopMoving()
    {
        animator.SetFloat(speedHash, 0f);
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void SetMoveTarget(Vector3 position)
    {
        moveTarget = position;
    }

    private void UpdateAnimator()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat(speedHash, speed);
    }
}