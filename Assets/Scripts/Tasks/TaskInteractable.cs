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

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject uiCanvas;

    public void Update()
    {
       if(task.Complete)
        {
            uiCanvas.SetActive(false);
        }
    }


    public void StartProgressBar(float time)
    {
        progressBar.StartProgressBar(time);
    }
    
    public void StopProgressBar()
    {
        progressBar.StopProgressBar();
    }
    public void ShowUI()
    {
        if (!task.Complete)
        {
            uiCanvas.SetActive(true);
        }
    }

    public void HideUI()
    {
        uiCanvas.SetActive(false);
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
