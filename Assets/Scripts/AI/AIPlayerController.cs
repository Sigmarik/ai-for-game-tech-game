using UnityEngine;
using UnityEngine.AI;

public class AIPlayerController : MonoBehaviour
{
    /// <summary>
    /// Define AIPlayer Ability in Controller
    /// Using State to switch
    /// </summary>
    
    // Properties
    [SerializeField] private float moveSpeed = 3f;

    public Vector3 moveTarget { get; private set; }

    public Animator animator;
    private static readonly int speedHash = Animator.StringToHash("MoveSpeed");
    public AIStateMachine StateMachine { get; private set; }

    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public InteractState InteractState { get; private set; }
    public AIBrain Brain { get; private set; }


    // Nav
    private NavMeshAgent agent;


    //Debug
    [SerializeField] private string currentStateName;

    [SerializeField] public Transform testTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        agent.speed = moveSpeed;

        StateMachine = new AIStateMachine();

        IdleState = new IdleState(this, StateMachine);
        MoveState = new MoveState(this, StateMachine);
        InteractState = new InteractState(this, StateMachine);

        Brain = new AIBrain(this);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
        Brain.RequestWork();
    }

    private void Update()
    {
        StateMachine.Update();

        UpdateAnimator();

        currentStateName = StateMachine.CurrentState?.GetType().Name;
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