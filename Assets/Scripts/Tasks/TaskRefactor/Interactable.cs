using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;
    public event Action CheckTasks;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject uiCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartInteraction()
    {
        OnInteractionStart.Invoke();
    }

    public void StopInteraction()
    {
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
