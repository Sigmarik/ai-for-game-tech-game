using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIBrain
{
    /// <summary>
    ///  Tell AI what gonna do next
    /// </summary>
    AIPlayerController controller;
    Task currentTask;
    public AIBrain(AIPlayerController controller)
    {
        this.controller = controller;
    }

    public void RequestWork()
    {
        // var target = controller.testTarget.position;
        // controller.SetMoveTarget(target);
        // controller.StateMachine.ChangeState(controller.MoveState);
        TaskManager.instance.GetCompletableTasks();
        List<Task> tasks = TaskManager.instance.CompletableTasks;
        if(tasks.Count == 0)
        {
            //Debug.Log("Null?");
            controller.StateMachine.ChangeState(controller.IdleState);
            return;
        }
        else
        {
            //Debug.Log("have something to do");
            // default pickup the first one, will optimize it later
            currentTask = tasks[0];
            controller.SetMoveTarget(currentTask.interactable.transform.position);
            controller.StateMachine.ChangeState(controller.MoveState);
        }
    }

    public void OnMoveComplete()
    {
        controller.StateMachine.ChangeState(controller.InteractState);
    }

    public void OnInteractComplete()
    {
        RequestWork();
    }

    public void StartInteraction()
    {
        currentTask.interactable.ShowUI(); 
        currentTask.interactable.StartInteraction();
    }

    public bool IsStepDone()
    {
        if (currentTask.IsComplete)
        {
            return true;
        }
        return false;
    }
}
