using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    private bool isComplete = false;
    private Coroutine stepCoroutine;

    public void Start()
    {
        stepIndex = 0;
        foreach (TaskStepDefinition step in taskDef.taskSteps)
        {
            taskSteps.Add(step.Instatiate(this));
        }
        foreach (TaskInteractable interactable in interactables)
        {
            Debug.Log("We are setting the task");
            interactable.Task = this;
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
            isComplete = true;
            // TODO: Completion Logic
        }
    }
    
    public void InteractionStarted(TaskInteractable interactable)
    {
        Debug.Log(isComplete);
        if (isComplete) return;
        if (CurrentStep.CheckInteractable(interactable.interactableType))
            InitiateCurrentTaskStep(interactable);
    }

    public void InteractionStopped()
    {
        if (isComplete) return;
        if (CurrentStep.IsInitiated)
        {
            // TODO: If the step should be stopped has to made dependant on if the step is autonomous or not
            Debug.Log("Step Stopped");
            StopCoroutine(stepCoroutine);
            CurrentStep.StopStep();
        }
    }

    public void InitiateCurrentTaskStep(TaskInteractable interactable)
    {
        if (!(0 <= stepIndex && stepIndex < taskDef.taskSteps.Count))
            return;

        if (!CurrentStep.IsInitiated && CurrentStep.requirements.All((TaskStepRequirement req) => req.RequirementMet()))
            stepCoroutine = StartCoroutine(CurrentStep.StartStep(interactable));
    }
}
