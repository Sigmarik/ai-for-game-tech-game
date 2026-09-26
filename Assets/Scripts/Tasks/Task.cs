using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "Tasks/Task")]
public class TaskDefinition : ScriptableObject
{
    public string taskName;
    public List<TaskStep> taskSteps = new List<TaskStep>();
}

public class TaskInstance : MonoBehaviour
{
    public int stepIndex;
    public TaskDefinition task;

    public void Initialize(TaskDefinition taskDef)
    {
        stepIndex = 0;
        task = taskDef; 
    }

    public void IncrementStepIndex()
    {

    }
    
    public TaskStep InitiateCurrentTaskStep()
    {
        if (!(0 <= stepIndex && stepIndex < task.taskSteps.Count))
            return null;

        TaskStep taskStep = task.taskSteps[stepIndex];
        if (!taskStep.IsInitiated && taskStep.requirements.All((TaskStepRequirement req) => req.RequirementMet()))
        {
            StartCoroutine(taskStep.StartStep());
            return taskStep;
        }
        else return null;
    }
}

