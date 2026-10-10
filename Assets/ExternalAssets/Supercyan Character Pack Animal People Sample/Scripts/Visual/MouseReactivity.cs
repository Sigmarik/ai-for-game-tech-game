using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseReactivity : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera virtualCamera;
    private Quaternion originalLocalRotation;

    [Header("Mouse Reactivity")]
    [Tooltip("Maximum rotation angle (degrees) when the mouse is at the screen edge")]
    public float maxRotationAngle = 5f;

    [Tooltip("How fast the camera interpolates toward the target rotation")]
    public float smoothSpeed = 5f;

    [Tooltip("Invert the vertical mouse response")]
    public bool invertY = false;

    private Quaternion targetLocalRotation;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        originalLocalRotation = virtualCamera.transform.localRotation;
        targetLocalRotation = originalLocalRotation;
    }

    // LateUpdate so we apply the offset AFTER Cinemachine has updated the vcam transform
    void LateUpdate()
    {
        // Normalize mouse position to [-1, 1] relative to screen center
        Vector3 mousePos = Input.mousePosition;
        float normX = Mathf.Clamp((mousePos.x / Screen.width) * 2f - 1f, -1f, 1f);
        float normY = Mathf.Clamp((mousePos.y / Screen.height) * 2f - 1f, -1f, 1f);

        if (invertY) normY = -normY;

        // Compute the local rotation offset
        // mouse right -> yaw right (+), mouse up -> pitch up (-)
        float yaw = normX * maxRotationAngle;
        float pitch = -normY * maxRotationAngle;

        // Base rotation (captured at start) + offset applied in local space
        targetLocalRotation = originalLocalRotation * Quaternion.Euler(pitch, yaw, 0f);

        // Smoothly interpolate toward the target
        virtualCamera.transform.localRotation = Quaternion.Slerp(
            virtualCamera.transform.localRotation,
            targetLocalRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}
