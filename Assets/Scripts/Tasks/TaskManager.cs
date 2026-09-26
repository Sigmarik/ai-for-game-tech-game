using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;
    private List<TaskInstance> tasks = new List<TaskInstance>();
    private Dictionary<TaskStep, TaskInstance> activeTasks = new Dictionary<TaskStep, TaskInstance>(); 

    public TaskManager()
    {
        if (instance == null)
            instance = new TaskManager();
    }

    public void AddTask(TaskDefinition taskDef)
    {
        tasks.Add(new TaskInstance(taskDef));
    }

    public void ActivateTaskStep(TaskInstance task)
    {
        TaskStep step = task.InitiateCurrentTaskStep();
        if (step != null)
            activeTasks[step] = task;
    }

    public void StepComplete(TaskStep step)
    {
        if (!activeTasks.ContainsKey(step))
            return;

        activeTasks[step].IncrementStepIndex();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
