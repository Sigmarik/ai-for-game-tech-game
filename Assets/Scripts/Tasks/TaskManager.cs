using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private List<Task> endTasks = new List<Task>();
    private List<Task> completableTasks;
    public List<Task> CompletableTasks { get { return completableTasks; } }
    private bool finished;
    public bool Finished { get { return finished; } }

    [SerializeField] private bool activateOnStart = false;

    private HashSet<Task> processedTasks = new HashSet<Task>();
    private bool isInitialized = false;

    private void Awake()
    {
    }

    void Start()
    {
        if (activateOnStart) MakeActive();
    }

    void Update()
    {
    }

    public void MakeActive()
    {
        if (isInitialized) return;
        isInitialized = true;

        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null)
            {
                endTasks.Add(childTask);
                MakeTasksActive(childTask);
                SubToEndTaskCompletion(childTask);
            }
        }
        GetCompletableTasks();

        ExperimentLogger.LogEvent("TASK_MANAGER", "activated",
            "object", gameObject.name,
            "endTaskCount", endTasks.Count.ToString());
    }

    public void MakeTasksActive(Task task)
    {
        if (processedTasks.Contains(task)) return;
        processedTasks.Add(task);

        task.MakeActive();
        task.OnTaskComplete += TaskCompletes;
        foreach (Task childTask in task.ReqTasks)
            MakeTasksActive(childTask);
    }

    public void TaskCompletes(Task task)
    {
        task.interactable.HideUI();
        task.interactable.SetHighlight(false);
        task.interactable.SetActive(false);
        GetCompletableTasks();
    }

    public void SubToEndTaskCompletion(Task task)
    {
        task.OnTaskComplete += EndTaskCompletes;
    }

    public void EndTaskCompletes(Task task)
    {
        if (endTasks.All(endTask => endTask.IsComplete))
        {
            finished = true;
            ExperimentLogger.LogEvent("TASK_MANAGER", "all tasks finished",
                "object", gameObject.name,
                "car", transform.root.name);
        }
    }

    public void GetCompletableTasks()
    {
        List<Task> _completableTasks = new List<Task>();
        foreach (Task task in endTasks)
            _completableTasks = _completableTasks.Concat(CompletableOrChildren(task)).ToList();

        if (completableTasks != null)
        {
            foreach (Task task in completableTasks)
            {
                if (!_completableTasks.Contains(task))
                {
                    task.interactable.SetActive(false);
                }
            }
        }

        completableTasks = _completableTasks;
        foreach (Task task in completableTasks)
        {
            task.interactable.SetActive(true);
            task.interactable.ForceUpdate();
        }
    }

    public List<Task> CompletableOrChildren(Task task)
    {
        if (task.IsComplete)
            return new List<Task>();
        if (task.AllRequirements())
            return new List<Task>() { task };
        List<Task> completableTasks = new List<Task>();
        foreach (Task reqTask in task.ReqTasks)
            completableTasks = completableTasks.Concat(CompletableOrChildren(reqTask)).ToList();
        return completableTasks;
    }
}