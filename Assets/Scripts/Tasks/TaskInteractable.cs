using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Tire
}

public class TaskInteractable : MonoBehaviour
{
    [SerializeField]
    public InteractableType interactableType;
    private TaskInstance task;
    public TaskInstance Task
    {
        get { return task; }
        set { if (task == null) { task = value; } }
    }

    // TESTING VARIABLES
    // TODO: REMOVE THESE
    bool wasPressed = false;

    public void Update()
    {
        if (Input.GetKeyDown("space") && !wasPressed)
        {
            wasPressed = true;
            StartInteraction();
        }
        else if (Input.GetKeyUp("space") && wasPressed)
        {
            wasPressed = false;
            StopInteraction();
        }
    }

    public void StartInteraction()
    {
        task.InteractionStarted(interactableType);
    }

    public void StopInteraction()
    {
        task.InteractionStopped();
    }
}
