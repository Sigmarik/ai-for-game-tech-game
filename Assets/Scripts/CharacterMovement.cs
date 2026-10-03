using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_visual;

    [Header("Movement")]
    [SerializeField] private float m_moveSpeed = 6f;
    [SerializeField] private float m_acceleration = 60f;
    [SerializeField] private float m_deceleration = 80f;
    [SerializeField] private float m_turnSpeed = 900f;

    [Header("Gravity")]
    [SerializeField] private float m_gravity = 25f;
    [SerializeField] private float m_fallMultiplier = 1.7f;

    [Header("Ground")]
    [SerializeField] private LayerMask m_groundLayers = ~0;

    [Header("Animation")]
    [SerializeField] private string m_moveSpeedParameter = "MoveSpeed";
    [SerializeField] private string m_groundedParameter = "Grounded";

    private Rigidbody m_rigidBody;
    private int m_moveSpeedHash;
    private int m_groundedHash;

    private readonly List<Vector3> m_wallNormals = new List<Vector3>(8);

    private Vector3 m_moveInput;
    private bool m_isGrounded;
    private bool m_groundContactThisStep;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        if (m_animator == null) m_animator = GetComponentInChildren<Animator>();

        if (m_visual == null) Debug.LogWarning($"{nameof(CharacterMovement)} on '{name}' has no visual assigned; the character will not turn.", this);

        m_rigidBody.isKinematic = false;
        m_rigidBody.useGravity = false;
        m_rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        m_rigidBody.sleepThreshold = 0f;
        m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        m_moveSpeedHash = Animator.StringToHash(m_moveSpeedParameter);
        m_groundedHash = Animator.StringToHash(m_groundedParameter);
    }

    public void Move(Vector3 direction)
    {
        m_moveInput = direction;
    }

    private void Update()
    {
        // Do nothing, send movement inputs from other components such as player/AI controllers.
    }

    private void OnCollisionStay(Collision collision)
    {
        bool isGroundLayer = ((1 << collision.gameObject.layer) & m_groundLayers.value) != 0;
        int contactCount = collision.contactCount;

        for (int contactIndex = 0; contactIndex < contactCount; contactIndex++)
        {
            Vector3 contactNormal = collision.GetContact(contactIndex).normal;

            if (contactNormal.y > 0.5f)
            {
                if (isGroundLayer) m_groundContactThisStep = true;
            }
            else if (contactNormal.y > -0.5f)
            {
                m_wallNormals.Add(contactNormal);
            }
        }
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        m_isGrounded = m_groundContactThisStep;
        m_groundContactThisStep = false;

        ApplyMovement(deltaTime);
        FaceVelocity(deltaTime);
        ApplyGravity(deltaTime);
        UpdateAnimator();

        m_wallNormals.Clear();
    }

    private void ApplyMovement(float deltaTime)
    {
        Vector3 currentVelocity = m_rigidBody.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 targetVelocity = m_moveInput * m_moveSpeed;

        int wallCount = m_wallNormals.Count;
        for (int wallIndex = 0; wallIndex < wallCount; wallIndex++)
        {
            Vector3 wallNormal = m_wallNormals[wallIndex];

            if (Vector3.Dot(targetVelocity, wallNormal) < 0f)
            {
                targetVelocity = Vector3.ProjectOnPlane(targetVelocity, wallNormal);
            }
        }

        float accelerationRate = m_moveInput.sqrMagnitude > 0.01f ? m_acceleration : m_deceleration;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accelerationRate * deltaTime);

        m_rigidBody.velocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    private void FaceVelocity(float deltaTime)
    {
        if (m_visual == null) return;

        Vector3 currentVelocity = m_rigidBody.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        if (horizontalVelocity.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
        m_visual.rotation = Quaternion.RotateTowards(m_visual.rotation, targetRotation, m_turnSpeed * deltaTime);
    }

    private void ApplyGravity(float deltaTime)
    {
        Vector3 currentVelocity = m_rigidBody.velocity;

        float activeGravity = currentVelocity.y < 0f ? m_gravity * m_fallMultiplier : m_gravity;
        currentVelocity.y -= activeGravity * deltaTime;

        if (m_isGrounded && currentVelocity.y < 0f) currentVelocity.y = 0f;

        m_rigidBody.velocity = currentVelocity;
    }

    private void UpdateAnimator()
    {
        if (m_animator == null) return;

        Vector3 currentVelocity = m_rigidBody.velocity;
        float planarSpeed = new Vector2(currentVelocity.x, currentVelocity.z).magnitude;
        float normalizedSpeed = planarSpeed / Mathf.Max(m_moveSpeed, 0.0001f);

        m_animator.SetFloat(m_moveSpeedHash, Mathf.Clamp01(normalizedSpeed));
        m_animator.SetBool(m_groundedHash, m_isGrounded);
    }
}
