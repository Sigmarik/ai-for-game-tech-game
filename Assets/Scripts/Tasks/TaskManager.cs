using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;
    private List<Task> endTasks = new List<Task>();
    private List<Task> completableTasks;
    public List<Task> CompletableTasks { get { return completableTasks; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null)
            {
                endTasks.Add(childTask);
                SubToTaskCompletion(childTask);
            }
        }
        GetCompletableTasks();
    }

    // Start is called before the first frame update
    void Start()
    {
        // foreach (Transform child in transform)
        // {
        //     Task childTask = child.GetComponent<Task>();
        //     if (childTask != null)
        //     {
        //         endTasks.Add(childTask);
        //         SubToTaskCompletion(childTask);
        //     }
        // }
        // GetCompletableTasks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubToTaskCompletion(Task task)
    {
        task.OnTaskComplete += TaskCompletes;
        foreach (Task childTask in task.ReqTasks)
            SubToTaskCompletion(childTask);
    }

    public void TaskCompletes(Task task)
    {
        GetCompletableTasks();
    }

    public void GetCompletableTasks()
    {
        List<Task> _completableTasks = new List<Task>();
        foreach (Task task in endTasks)
            _completableTasks = _completableTasks.Concat(CompletableOrChildren(task)).ToList();
        completableTasks = _completableTasks;
        foreach (Task task in completableTasks)
            Debug.Log(task.name);
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
