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

    private ProgressBar progressBar;

    // TESTING VARIABLES
    // TODO: REMOVE THESE
    bool wasPressed = false;

    public void Start()
    {
        progressBar = GetComponentInChildren<ProgressBar>();
    }

    public void StartProgressBar(float time)
    {
        progressBar.StartProgressBar(time);
    }
    
    public void StopProgressBar()
    {
        progressBar.StopProgressBar();
    }

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
        task.InteractionStarted(this);
    }

    public void StopInteraction()
    {
        task.InteractionStopped();
    }

    public void NextState()
    {
        // TODO: make this an actual state machine
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = !renderer.enabled;
    }
}
