using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;
    public event Action CheckTasks;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject uiCanvas;

    private int taskCount = 0;

    public Task ActiveTask {get; private set;} // add this so Interactable will know which task is active
    public void SetActiveTask(Task task) {ActiveTask = task;}
    public void ClearActiveTask(Task task) {if(ActiveTask == task) ActiveTask  = null; }

    public object ActiveOwner {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (taskCount <= 0)
            uiCanvas.SetActive(false);
    }

    public void TaskSubscribed()
    {
        taskCount++;
    }

    public void TaskUnsubscribed()
    {
        taskCount--;
        if (taskCount <= 0)
        {
            uiCanvas.SetActive(false);
            gameObject.SetHighlight(false);
        }
    }

    private void StartInteraction()
    {
        if (OnInteractionStart != null)
            OnInteractionStart.Invoke();
    }

    private void StopInteraction()
    {
        if (OnInteractionEnd != null)
            OnInteractionEnd.Invoke();
    }

    public bool TryStartInteraction(object owner) // AI funtion
    {
        if (ActiveOwner != null && ActiveOwner != owner)
            return false;
        ActiveOwner = owner;
        StartInteraction();
        return true;
    }

    public void ForceStartInteraction(object owner) // Player Function
    {
        if (ActiveOwner != null && ActiveOwner != owner)
            StopInteraction(); 
        ActiveOwner = owner;
        StartInteraction();
        
    }

    public void StopInteraction(object owner) // Player Function
    {
        if (ActiveOwner != owner)
            return;
        ActiveOwner = null;
        StopInteraction();
    }

    public void NextState()
    {
        // TODO: make this an actual state machine
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = !renderer.enabled;
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
        if (taskCount <= 0)
            return;
        uiCanvas.SetActive(true);
        gameObject.SetHighlight(true);
    }
    public void HideUI()
    {
        uiCanvas.SetActive(false);
        gameObject.SetHighlight(false);
    }
}
