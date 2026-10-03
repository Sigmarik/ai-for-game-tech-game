using System;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private float distance;
    private TaskInteractable interactable;
    private TaskInteractable oldInteractable;
    private bool started = false;
    private bool uiOpen = false;

    [SerializeField] private bool showUI = false;

    private bool wantsToInteract = false;

    public void SetInteracting(bool interacting)
    {
        wantsToInteract = interacting;
    }

    void Update()
    {
        TaskInteractable closestInteractable = FindClosestInteractable();

        if (closestInteractable != interactable)
        {
            oldInteractable = interactable;
            interactable = closestInteractable;

            if (uiOpen && oldInteractable != null)
            {
                oldInteractable.HideUI();
                uiOpen = false;
            }

            if (started && oldInteractable != null)
            {
                oldInteractable.StopInteraction();
                started = false;
            }
        }

        if (interactable != null && !uiOpen)
        {
            if (showUI) interactable.ShowUI();
            uiOpen = true;
        }

        if (interactable != null && wantsToInteract && !started)
        {
            interactable.StartInteraction();
            started = true;
        }
        else if (interactable == null && started)
        {
            if (oldInteractable != null)
            {
                oldInteractable.StopInteraction();
            }
            started = false;
        }
        else if (interactable != null && !wantsToInteract && started)
        {
            interactable.StopInteraction();
            started = false;
        }
    }

    public List<TaskInteractable> FindAllInteractables()
    {
        TaskInteractable[] allInteractables = FindObjectsByType<TaskInteractable>(FindObjectsSortMode.None);
        List<TaskInteractable> interactablesInRange = new List<TaskInteractable>();

        Vector3 originPosition = rayOrigin.transform.position;

        foreach (TaskInteractable currentInteractable in allInteractables)
        {
            if (currentInteractable == null)
            {
                continue;
            }

            float currentDistance = Vector3.Distance(originPosition, currentInteractable.transform.position);
            if (currentDistance <= distance)
            {
                interactablesInRange.Add(currentInteractable);
            }
        }

        return interactablesInRange;
    }

    public TaskInteractable FindClosestInteractable()
    {
        TaskInteractable[] allInteractables = FindObjectsOfType<TaskInteractable>();
        TaskInteractable bestInteractable = null;
        float bestDistance = distance;
        Vector3 originPosition = rayOrigin.transform.position;

        foreach (TaskInteractable currentInteractable in allInteractables)
        {
            if (currentInteractable == null)
            {
                continue;
            }

            float currentDistance = Vector3.Distance(originPosition, currentInteractable.transform.position);
            if (currentDistance <= bestDistance)
            {
                bestDistance = currentDistance;
                bestInteractable = currentInteractable;
            }
        }

        if (bestDistance > distance)
        {
            bestInteractable = null;
        }

        return bestInteractable;
    }
}
