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

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (activateOnStart) MakeActive();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MakeActive()
    {
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
    }
    public void MakeTasksActive(Task task)
    {
        task.MakeActive();
        task.interactable.SetActive(true);
        task.OnTaskComplete += TaskCompletes;
        foreach (Task childTask in task.ReqTasks)
            MakeTasksActive(childTask);
    }

    public void TaskCompletes(Task task)
    {
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
        task.interactable.SetActive(false);
        if (endTasks.All(task => task.IsComplete))
            finished = true;
    }

    public void GetCompletableTasks()
    {
        List<Task> _completableTasks = new List<Task>();
        foreach (Task task in endTasks)
            _completableTasks = _completableTasks.Concat(CompletableOrChildren(task)).ToList();
        completableTasks = _completableTasks;
        foreach (Task task in completableTasks)
        {
            Debug.Log(task.name);
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
