using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TaskInstance : MonoBehaviour
{
    [SerializeField]
    public TaskDefinition taskDef;
    public List<TaskInteractable> interactables;

    private int stepIndex;
    private List<TaskStepInstance> taskSteps = new List<TaskStepInstance>();
    public TaskStepInstance CurrentStep
    {
        get { return taskSteps[stepIndex]; }
    }

    public void Initialize()
    {
        stepIndex = 0;
        foreach (TaskStepDefinition step in taskDef.taskSteps)
        {
            taskSteps.Add(step.Instatiate(this));
        }
    }

    public void CompleteStep(TaskStepInstance step)
    {
        if (step != CurrentStep)
            return;
        CompleteCurrentStep();
    }

    private void CompleteCurrentStep()
    {
        stepIndex++;
        if (stepIndex >= taskDef.taskSteps.Count)
        {
            // TODO: Completion Logic
        }
    }
    
    public void InitiateCurrentTaskStep()
    {
        if (!(0 <= stepIndex && stepIndex < taskDef.taskSteps.Count))
            return;

        TaskStepInstance taskStep = taskSteps[stepIndex];
        if (!taskStep.IsInitiated && taskStep.requirements.All((TaskStepRequirement req) => req.RequirementMet()))
            StartCoroutine(taskStep.StartStep());
    }

    public void InteractionStarted(InteractableType interactableType)
    {
        if (CurrentStep.CheckInteractable(interactableType))
            InitiateCurrentTaskStep();
    }
}
