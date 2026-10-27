using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class Task : MonoBehaviour
{
    [SerializeField] public Interactable interactable;
    [SerializeField] public GameObject returnItem;
    [SerializeField] public float interactDuration;
    [SerializeField] public List<Task> adoptedRequiredTasks = new List<Task>(); 
    [SerializeField] public TextMeshProUGUI labelToChange;

    private List<Task> reqTasks = new List<Task>();
    public List<Task> ReqTasks { get { return reqTasks; } }
    private bool isComplete = false;
    public bool IsComplete { get { return isComplete; } }
    private Coroutine activeTask;

    public event Action<Task> OnTaskComplete;

    void Start()
    {
        OnTaskComplete += OnTaskEnd;
    }

    public virtual void OnTaskEnd(Task task)
    {

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
    }

    public void Activate()
    {
        if (isComplete || !AllRequirements())
            return;
        interactable.SetActiveTask(this);
        activeTask = StartCoroutine(this.ActivateTask());
    }

    public void Deactivate()
    {
        if (activeTask != null)
        {
            interactable.ClearActiveTask(this);
            StopCoroutine(activeTask);
            interactable.StopProgressBar();
            activeTask = null;
        }
    }

    public bool AllRequirements()
    {
        if (reqTasks.Count == 0 || reqTasks.All(task => task.IsComplete))
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
        interactable.ClearActiveTask(this);

        var interacter = interactable.CurrentInteracter;
        if (interacter != null)
        {
            var aiController = interacter.GetComponent<AIPlayerController>();
            if (aiController == null)
            {
                PlayerTaskTracker.LastPlayerTask = this;
            }
        }

        interactable.StopInteraction();
        isComplete = true;
        if (OnTaskComplete != null)
            OnTaskComplete.Invoke(this);
    }
}