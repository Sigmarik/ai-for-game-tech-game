using System;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private float distance;
    private Interactable interactable;
    public Interactable CurrentInteractable => interactable;
    private Interactable oldInteractable;
    private Interactable selectedInteractable;
    private bool started = false;
    public bool IsInteracting => started;
    private bool uiOpen = false;

    [SerializeField] private bool showUI = false;

    private bool wantsToInteract = false;

    public void SetInteracting(bool interacting)
    {
        wantsToInteract = interacting;
    }

    public bool IsInteractable(Interactable other)
    {
        return interactable == other;
    }

    void Update()
    {
        Interactable closestInteractable = FindClosestInteractable();

        if (closestInteractable != interactable)
        {
            if (interactable != null)
            {
                if (started)
                {
                    interactable.StopInteraction();
                    started = false;
                }
                if (uiOpen)
                {
                    interactable.HideUI();
                    uiOpen = false;
                }
            }

            oldInteractable = interactable;
            interactable = closestInteractable;
        }

        Interactable desiredSelection = (interactable != null && interactable.IsActive) ? interactable : null;
        if (desiredSelection != selectedInteractable)
        {
            if (selectedInteractable != null)
            {
                selectedInteractable.RemoveSelector();
            }
            selectedInteractable = desiredSelection;
            if (selectedInteractable != null)
            {
                selectedInteractable.AddSelector();
            }
        }

        bool shouldShowUI = interactable != null && interactable.IsActive && showUI;
        if (shouldShowUI && !uiOpen)
        {
            interactable.ShowUI();
            uiOpen = true;
        }
        else if (!shouldShowUI && uiOpen)
        {
            if (interactable != null)
            {
                interactable.HideUI();
            }
            uiOpen = false;
        }

        if (interactable != null && wantsToInteract && !started)
        {
            started = interactable.StartInteraction(this);
        }
        else if (started && (!wantsToInteract || interactable == null || !interactable.IsActive))
        {
            if (interactable != null)
            {
                interactable.StopInteraction();
            }
            else if (oldInteractable != null)
            {
                oldInteractable.StopInteraction();
            }
            started = false;
        }
    }

    public List<Interactable> FindAllInteractables()
    {
        Interactable[] allInteractables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        List<Interactable> interactablesInRange = new List<Interactable>();

        Vector3 originPosition = rayOrigin.transform.position;

        foreach (Interactable currentInteractable in allInteractables)
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

    public Interactable FindClosestInteractable()
    {
        Interactable[] allInteractables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        Interactable bestInteractable = null;
        float bestDistance = distance;
        Vector3 originPosition = rayOrigin.transform.position;

        foreach (Interactable currentInteractable in allInteractables)
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

    public Task GetCurrentTask()
    {
        if (interactable == null || !started)
        {
            return null;
        }
        return interactable.ActiveTask;
    }

    public void OnInteractionStoppedExternally()
    {
        started = false;
    }
}