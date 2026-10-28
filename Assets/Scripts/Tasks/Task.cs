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
    private bool isActive = false;
    private Interact previousInteracter = null;
    private bool abortLogged = false;

    public event Action<Task> OnTaskComplete;

    void Start()
    {
        OnTaskComplete += OnTaskEnd;
    }

    void Update()
    {
        if (!isActive || isComplete || interactable == null) return;

        if (!interactable.IsActive)
        {
            if (activeTask != null && previousInteracter != null && !abortLogged)
            {
                ExperimentLogger.LogEvent("TASK", "aborted by interactable deactivation",
                    "task", name,
                    "interactable", interactable.name,
                    "by", previousInteracter.gameObject.name);
                abortLogged = true;
            }
            return;
        }

        bool isBeingInteractedWith = interactable.Interacter != null;

        if (isBeingInteractedWith)
        {
            previousInteracter = interactable.Interacter;

            if (interactable.ActiveTask == null && AllRequirements())
            {
                interactable.SetActiveTask(this);
            }

            if (interactable.ActiveTask == this)
            {
                if (activeTask == null && AllRequirements())
                {
                    activeTask = StartCoroutine(ActivateTask());
                }
            }
        }
        else
        {
            if (activeTask != null && !abortLogged)
            {
                string aborterName = previousInteracter != null ? previousInteracter.gameObject.name : "unknown";
                ExperimentLogger.LogEvent("TASK", "aborted by lost interaction",
                    "task", name,
                    "interactable", interactable.name,
                    "by", aborterName);
                abortLogged = true;
            }

            if (interactable.ActiveTask == this)
            {
                interactable.ClearActiveTask(this);
            }

            if (activeTask != null)
            {
                StopCoroutine(activeTask);
                interactable.StopProgressBar();
                activeTask = null;
            }
        }
    }

    public virtual void OnTaskEnd(Task task)
    {
    }

    public void MakeActive()
    {
        if (isActive) return;
        isActive = true;

        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null) reqTasks.Add(childTask);
        }
        foreach (Task adoptedTask in adoptedRequiredTasks)
            reqTasks.Add(adoptedTask);
        interactable.TaskSubscribed();
    }

    public bool AllRequirements()
    {
        if (reqTasks.Count == 0 || reqTasks.All(task => task.IsComplete))
            return true;
        return false;
    }

    public IEnumerator ActivateTask()
    {
        abortLogged = false;

        var interacter = interactable.Interacter;
        if (interacter != null && interacter.GetComponent<PlayerController>() != null)
        {
            PlayerTaskTracker.LastPlayerTask = this;
        }

        GenericController controller = interacter != null ? interacter.GetComponent<GenericController>() : null;
        string interacterType = controller is AIPlayerController ? "ai"
            : controller != null ? "player" : "unknown";
        string interacterName = interacter != null ? interacter.gameObject.name : "unknown";

        ExperimentLogger.LogEvent("TASK", "started",
            "task", name,
            "interactable", interactable.name,
            "by", interacterName,
            "type", interacterType,
            "duration", interactDuration.ToString("F2"));

        interactable.StartProgressBar(interactDuration);
        yield return new WaitForSeconds(interactDuration);

        if (interactable.ActiveTask == this && interactable.Interacter != null)
        {
            interactable.StopProgressBar();
            interactable.NextState();

            isComplete = true;
            interactable.StopInteraction();
            interactable.ClearActiveTask(this);
            interactable.TaskUnsubscribed();

            ExperimentLogger.LogEvent("TASK", "completed",
                "task", name,
                "interactable", interactable.name,
                "by", interacterName,
                "type", interacterType);

            activeTask = null;
            if (OnTaskComplete != null)
                OnTaskComplete.Invoke(this);
        }
        else
        {
            interactable.StopProgressBar();
            interactable.ClearActiveTask(this);
            activeTask = null;
        }
    }
}