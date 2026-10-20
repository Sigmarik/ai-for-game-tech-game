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

    private MeshRenderer outline;

    private Interact interacter = null;

    private bool active = true;

    private void Start()
    {
        GameObject obj = new GameObject("outline");
        outline = obj.AddComponent<MeshRenderer>();
        outline.SetMaterials(new List<Material>());
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        filter.mesh = GetComponent<MeshFilter>().mesh;
        obj.transform.SetParent(transform, false);
        obj.transform.localRotation = Quaternion.Euler(0, 0, 0); 
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        if (taskCount <= 0)
            uiCanvas.SetActive(false);
    }

    public void TaskSubscribed()
    {
        if (!active) return;
        taskCount++;
    }

    public void TaskUnsubscribed()
    {
        if (!active) return;
        taskCount--;
        if (taskCount <= 0)
        {
            uiCanvas.SetActive(false);
            SetHighlight(false);
        }
    }

    public void StartInteraction(Interact p_interacter)
    {
        if (!active) return;
        interacter = p_interacter;
        if (OnInteractionStart != null)
            OnInteractionStart.Invoke();
    }

    public void StopInteraction()
    {
        if (!active) return;
        interacter = null;
        if (OnInteractionEnd != null)
            OnInteractionEnd.Invoke();
    }

    public void NextState()
    {
        if (!active)
            return;
        // TODO: make this an actual state machine
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = !renderer.enabled;
    }

    public void ForceUpdate()
    {
        if (!active)
            return;
        if (interacter != null && interacter.IsInteractable(this))
        {
            SetHighlight(true, Color.white, 5f);
        }
        else
        {
            SetHighlight(true, Color.yellow, 3f);
        }
    }

    public void StartProgressBar(float time)
    {
        if (!active) return;
        progressBar.StartProgressBar(time);
    }
    public void StopProgressBar()
    {
        if (!active) return;
        progressBar.StopProgressBar();
    }
    public void ShowUI()
    {
        if (!active) return;
        if (taskCount <= 0)
            return;
        uiCanvas.SetActive(true);
        SetHighlight(true, Color.white, 5f);
    }
    public void HideUI()
    {
        if (!active) return;
        uiCanvas.SetActive(false);
        SetHighlight(true, Color.yellow, 3f);
    }

    public void SetHighlight(bool on,  Color? color = null, float? thickness = null)
    {
        if (!active) return;
        outline.gameObject.SetHighlight(on, color, thickness);
    }

    public void SetActive(bool p_active)
    {
        active = p_active;
    }
}
