using System.Collections.Generic;
using UnityEngine;

public class AIBrain
{
    AIPlayerController controller;
    public Task currentTask;

    public AIBrain(AIPlayerController controller)
    {
        this.controller = controller;
    }

    public void RequestWork()
    {
        List<Task> tasks = new List<Task>();
        TaskManager[] managers = Object.FindObjectsByType<TaskManager>(FindObjectsSortMode.None);
        foreach (TaskManager manager in managers)
        {
            if (manager.CompletableTasks != null)
                tasks.AddRange(manager.CompletableTasks);
        }

        if (tasks.Count == 0)
        {
            controller.StateMachine.ChangeState(controller.IdleState);
            return;
        }

        Task playerLastTask = PlayerTaskTracker.LastPlayerTask;
        if (playerLastTask != null && !IsTaskValid(playerLastTask))
        {
            playerLastTask = null;
            PlayerTaskTracker.LastPlayerTask = null;
        }

        if (playerLastTask == null)
        {
            Vector3 aiPos = controller.transform.position;
            tasks.Sort((a, b) =>
            {
                float da = Vector3.Distance(aiPos, a.interactable.transform.position);
                float db = Vector3.Distance(aiPos, b.interactable.transform.position);
                return da.CompareTo(db);
            });
        }
        else
        {
            var distances = TaskGraph.GetDistancesFrom(playerLastTask);
            bool furthest = controller.TaskSchedulingMode == AITaskSchedulingMode.Furthest;
            tasks.Sort((a, b) =>
            {
                int da = distances.TryGetValue(a, out int d1) ? d1 : int.MaxValue;
                int db = distances.TryGetValue(b, out int d2) ? d2 : int.MaxValue;
                if (furthest)
                    return db.CompareTo(da);
                return da.CompareTo(db);
            });
        }

        foreach (Task task in tasks)
        {
            if (!AITaskCoordinator.IsClaimByOthers(task, this))
            {
                AITaskCoordinator.TryClaimTask(task, this);
                currentTask = task;
                controller.SetMoveTarget(currentTask.interactable.transform.position);
                controller.StateMachine.ChangeState(controller.MoveState);
                return;
            }
        }
        controller.StateMachine.ChangeState(controller.IdleState);
    }

    private bool IsTaskValid(Task task)
    {
        if (task == null) return false;
        if (!task.gameObject.activeInHierarchy) return false;
        var manager = task.GetComponentInParent<TaskManager>();
        if (manager == null || manager.CompletableTasks == null || manager.Finished) return false;
        return true;
    }

    public void OnMoveComplete()
    {
        controller.StateMachine.ChangeState(controller.InteractState);
    }

    public void OnInteractComplete()
    {
        if (currentTask != null)
        {
            AITaskCoordinator.ReleaseTask(currentTask, this);
            currentTask = null;
        }
        RequestWork();
    }

    public void StartInteraction()
    {
        currentTask.interactable.ShowUI(); 
        currentTask.interactable.StartInteraction(controller.m_interact);
    }

    public bool IsStepDone()
    {
        if (currentTask.IsComplete)
        {
            return true;
        }
        return false;
    }
    public bool IsCurrentTaskClaimedByPlayer()
    {
        return currentTask != null && AITaskCoordinator.IsClaimByOthers(currentTask, this);
    }

    public void AbandonCurrentTask()
    {
        if (currentTask != null)
        {
            if (currentTask.interactable.ActiveTask == currentTask)
                currentTask.interactable.StopInteraction();

            AITaskCoordinator.ReleaseTask(currentTask, this);
            currentTask = null;
        }
        RequestWork();
    }
}