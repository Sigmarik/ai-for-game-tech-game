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
            uiCanvas.SetActive(false);
    }

    public void StartInteraction()
    {
        if (OnInteractionStart != null)
            OnInteractionStart.Invoke();
    }

    public void StopInteraction()
    {
        if (OnInteractionEnd != null)
            OnInteractionEnd.Invoke();
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
        uiCanvas.SetActive(true);
        gameObject.SetHighlight(true);
    }
    public void HideUI()
    {
        uiCanvas.SetActive(false);
        gameObject.SetHighlight(false);
    }
}
