using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;
    private List<TaskInstance> tasks = new List<TaskInstance>();
    private Dictionary<TaskStep, TaskInstance> activeTasks = new Dictionary<TaskStep, TaskInstance>();

    [SerializeField]
    public TaskDefinition testTask;

    public void AddTask(TaskDefinition taskDef)
    {
        GameObject taskObject = new GameObject(taskDef.taskName);
        TaskInstance taskInstance = taskObject.AddComponent<TaskInstance>();

        taskInstance.Initialize(taskDef);

        tasks.Add(taskInstance);
    }

    public void ActivateTaskStep(TaskInstance task)
    {
        TaskStep step = task.InitiateCurrentTaskStep();
        if (step != null)
        {
            activeTasks[step] = task;
        }
    }

    public void StepComplete(TaskStep step)
    {
        if (!activeTasks.ContainsKey(step))
            return;

        activeTasks[step].IncrementStepIndex();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        AddTask(testTask);
        ActivateTaskStep(tasks[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
