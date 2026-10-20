using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GenericController
{
    [SerializeField] private Camera m_camera;
    [Tooltip("Child transform holding the character model. It is rotated to face the velocity.")]
    private Task claimedTask;

    private void Awake()
    {
        if (m_camera == null) m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(ToCameraRelative(rawInput));

        SetInteracting(Input.GetKey(KeyCode.F));
    }
    void LateUpdate()
    {
        UpdateTaskClaim();
    }

    private Vector3 ToCameraRelative(Vector2 rawInput)
    {
        if (rawInput.sqrMagnitude < 0.01f) return Vector3.zero;

        Vector3 cameraForward = Vector3.forward;
        Vector3 cameraRight = Vector3.right;

        if (m_camera != null)
        {
            cameraForward = m_camera.transform.forward;
            cameraRight = m_camera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward = cameraForward.sqrMagnitude < 0.001f ? Vector3.forward : cameraForward.normalized;
            cameraRight = cameraRight.sqrMagnitude < 0.001f ? Vector3.right : cameraRight.normalized;
        }

        Vector3 worldDirection = cameraForward * rawInput.y + cameraRight * rawInput.x;

        return worldDirection.normalized * Mathf.Clamp01(rawInput.magnitude);
    }

    private void UpdateTaskClaim()
    {
        if(m_interact == null) return;
        Task currentTask = m_interact.GetCurrentTask();

        if(claimedTask == currentTask) return; // player is still working on that

        ReleaseClaim(); // if not means Player finish it or give up

        if(currentTask != null)
        {
            AITaskCoordinator.ForceClaimTask(currentTask, this);
            claimedTask = currentTask;
        }

        
    }


    private void ReleaseClaim()
    {
        if(claimedTask == null) return;

        AITaskCoordinator.ReleaseTask(claimedTask, this);
        claimedTask = null;
    }
}
