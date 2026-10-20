using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    [SerializeField] public Interactable interactable;
    [SerializeField] public GameObject returnItem;
    [SerializeField] public float interactDuration;
    [SerializeField] public List<Task> adoptedRequiredTasks = new List<Task>(); // Tasks that cannot come from the prefab tree because they are children of another branch.

    [SerializeField] public Image label;
    [SerializeField] public Image labelCheck;
    [SerializeField] public Image labelProgress;

    private List<Task> reqTasks = new List<Task>();
    public List<Task> ReqTasks { get { return reqTasks; } }
    private bool isComplete = false;
    public bool IsComplete { get { return isComplete; } }
    private Coroutine activeTask;

    public event Action<Task> OnTaskComplete;

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null) reqTasks.Add(childTask);
        }
        foreach (Task adoptedTask in adoptedRequiredTasks)
            reqTasks.Add(adoptedTask);
        interactable.OnInteractionStart += Activate;
        interactable.OnInteractionEnd += Deactivate;
        interactable.TaskSubscribed();
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLabel(Image labelToActivate)
    {
        label.gameObject.SetActive(false);
        labelCheck.gameObject.SetActive(false);
        labelProgress.gameObject.SetActive(false);
        labelToActivate.gameObject.SetActive(true);
    }

    public void MakeActive()
    {
        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null) reqTasks.Add(childTask);
        }
        foreach (Task adoptedTask in adoptedRequiredTasks)
            reqTasks.Add(adoptedTask);
        interactable.OnInteractionStart += Activate;
        interactable.OnInteractionEnd += Deactivate;
        interactable.TaskSubscribed();

        label.gameObject.SetActive(true);
        labelCheck.gameObject.SetActive(false);
        labelProgress.gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (isComplete || !AllRequirements())
            return;
        activeTask = StartCoroutine(this.ActivateTask());
        SetLabel(labelProgress);
    }

    public void Deactivate()
    {
        if (activeTask != null)
        {
            StopCoroutine(activeTask);
            interactable.StopProgressBar();
            activeTask = null;
            SetLabel(label);
        }
    }

    public bool AllRequirements()
    {
        if (reqTasks.All(task => task.IsComplete))
            return true;
        return false;
    }

    public IEnumerator ActivateTask()
    {
        interactable.StartProgressBar(interactDuration);
        yield return new WaitForSeconds(interactDuration);
        interactable.StopProgressBar();
        interactable.NextState();
        interactable.OnInteractionStart -= Activate;
        interactable.OnInteractionEnd -= Deactivate;
        interactable.TaskUnsubscribed();
        isComplete = true;
        SetLabel(labelCheck);
        if (OnTaskComplete != null)
            OnTaskComplete.Invoke(this);
    }
}
