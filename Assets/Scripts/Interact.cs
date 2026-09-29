using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private float distance;
    private bool hitting = false;
    private bool oldHitting = false;
    private TaskInteractable interactable;
    private TaskInteractable oldInteractable;
    private bool started = false;
    private bool uiOpen = false;

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(rayOrigin.transform.position, rayOrigin.transform.forward);

        Debug.DrawRay(rayOrigin.transform.position, rayOrigin.transform.forward * distance, Color.black);

        if(Physics.Raycast(ray, out hit, distance, (1 << 3)))
        {
            if (!hitting) 
            {
                hitting = !hitting;
                oldHitting = !oldHitting;
                interactable = hit.collider.GetComponent<TaskInteractable>();
            }
            else if (hitting && interactable != hit.collider.GetComponent<TaskInteractable>())
            {
                oldInteractable = interactable;
                interactable = hit.collider.GetComponent<TaskInteractable>();
            }
        }
        else {
            if(hitting)
            {
                oldInteractable = interactable;
                interactable = null;
                oldHitting = hitting;
                hitting = !hitting;
            }
            else if(oldHitting)
            {
                oldHitting = hitting;
                oldInteractable = null;
            }
        }
        if(hitting && !uiOpen)
        {
            interactable.ShowUI();
            uiOpen = true;
        }
        if(!hitting && oldHitting && uiOpen)
        {
            oldInteractable.HideUI();
            uiOpen = false;
        }

        // Handle actual interactions based on raycast
        if (hitting && Input.GetKey(KeyCode.F) && !started)
        {
            interactable.StartInteraction();
            started = true;
        }
        else if (!hitting && oldHitting && started)
        {
            oldInteractable.StopInteraction();
            started = false;
        }
        else if (hitting && !Input.GetKey(KeyCode.F) && started)
        {
            interactable.StopInteraction();
            started = false;
        }

    }
}
