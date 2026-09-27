using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTaskStep", menuName = "Tasks/TaskStep")]
public class TaskStepDefinition : ScriptableObject
{
    public List<TaskStepRequirement> requirements;
    public bool autonomous;
    public float duration;
    public InteractableType requiredInteractableType;

    public TaskStepInstance Instatiate(TaskInstance parent)
    {
        TaskStepInstance stepInstance = new TaskStepInstance();
        stepInstance.Initialize(this, parent);
        return stepInstance;
    }
}

public class TaskStepInstance : MonoBehaviour 
{
    public GameObject interactable; // TODO: Make interactable class or something
    private TaskInstance parentTask;
    TaskStepDefinition stepDefinition;
    private bool isInitiated = false;
    public bool IsInitiated
    {
        get { return isInitiated;  }
        private set { isInitiated = value; }
    }
    public List<TaskStepRequirement> requirements
    {
        get { return stepDefinition.requirements; }
    }

    public void Initialize(TaskStepDefinition taskStepDef, TaskInstance parentTask)
    {
        this.stepDefinition = taskStepDef;
        this.parentTask = parentTask;
    }

    public bool CheckInteractable(InteractableType interactableType)
    {
        return interactableType == stepDefinition.requiredInteractableType;
    }

    public IEnumerator StartStep()
    {
        Debug.Log("Step Started");
        isInitiated = true;
        if (!stepDefinition.autonomous)
        {
            // TODO: Turn off player that initated step movement
        }
        yield return new WaitForSeconds(stepDefinition.duration);
        Debug.Log("Step completed");
        isInitiated = false;
        parentTask.CompleteStep(this);
    }

    public void StopStep()
    {
        isInitiated = false;
    }
}
