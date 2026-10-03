using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [Tooltip("Child transform holding the character model. It is rotated to face the velocity.")]

    private CharacterMovement m_characterMovement;
    private Interact m_interact;

    private void Awake()
    {
        if (m_camera == null) m_camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_characterMovement = GetComponent<CharacterMovement>();
        m_interact = GetComponent<Interact>();

        if (m_characterMovement == null) Debug.LogError($"{nameof(PlayerController)} on '{name}' has no {nameof(CharacterMovement)} component.", this);
        if (m_interact == null) Debug.LogError($"{nameof(PlayerController)} on '{name}' has no {nameof(Interact)} component.", this);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        m_characterMovement.Move(ToCameraRelative(rawInput));

        m_interact.SetInteracting(Input.GetKey(KeyCode.F));
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
}
