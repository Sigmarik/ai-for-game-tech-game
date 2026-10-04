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
    private AIStateMachine stateMachine;

    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }


    // Nav
    private NavMeshAgent agent;


    //Debug
    [SerializeField] private string currentStateName;

    [SerializeField] private Transform testTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        agent.speed = moveSpeed;

        stateMachine = new AIStateMachine();

        IdleState = new IdleState(this, stateMachine);
        MoveState = new MoveState(this, stateMachine);
    }

    private void Start()
    {
        stateMachine.Initialize(IdleState);
        
        moveTarget = testTarget.position;
        stateMachine.ChangeState(MoveState);
    }

    private void Update()
    {
        stateMachine.Update();

        UpdateAnimator();

        currentStateName = stateMachine.CurrentState?.GetType().Name;
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
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