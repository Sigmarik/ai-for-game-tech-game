using System.Collections.Generic;
using UnityEngine;

public class AIBrain
{
    AIPlayerController controller;
    public Task currentTask;

    private enum AIWorkOutcome { Unknown, ChoseTask, IdleNoTasks, IdleAllClaimed }
    private AIWorkOutcome m_lastLoggedOutcome = AIWorkOutcome.Unknown;

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
            if (m_lastLoggedOutcome != AIWorkOutcome.IdleNoTasks)
            {
                ExperimentLogger.LogEvent("AI_DECISION", "no completable tasks, going idle",
                    "ai", controller.name,
                    "mode", controller.TaskSchedulingMode.ToString());
                m_lastLoggedOutcome = AIWorkOutcome.IdleNoTasks;
            }
            controller.StateMachine.ChangeState(controller.IdleState);
            return;
        }

        Task playerLastTask = PlayerTaskTracker.LastPlayerTask;
        if (playerLastTask != null && !IsReferenceTaskValid(playerLastTask))
        {
            ExperimentLogger.LogEvent("AI_DECISION", "reference task invalidated",
                "ai", controller.name,
                "refTask", playerLastTask.name);
            playerLastTask = null;
            PlayerTaskTracker.LastPlayerTask = null;
        }

        string sortReason;
        if (playerLastTask == null)
        {
            sortReason = "distance_from_ai";
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
            sortReason = controller.TaskSchedulingMode == AITaskSchedulingMode.Furthest
                ? "distance_from_player_furthest"
                : "distance_from_player_closest";
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

        int skippedCount = 0;
        foreach (Task task in tasks)
        {
            if (!AITaskCoordinator.IsClaimByOthers(task, this))
            {
                AITaskCoordinator.TryClaimTask(task, this);
                currentTask = task;

                float distanceToTask = Vector3.Distance(controller.transform.position, currentTask.interactable.transform.position);

                ExperimentLogger.LogEvent("AI_DECISION", "chose task",
                    "ai", controller.name,
                    "task", task.name,
                    "interactable", task.interactable.name,
                    "mode", controller.TaskSchedulingMode.ToString(),
                    "reason", sortReason,
                    "refTask", playerLastTask == null ? "none" : playerLastTask.name,
                    "distance", distanceToTask.ToString("F2"),
                    "candidateCount", tasks.Count.ToString(),
                    "skipped", skippedCount.ToString());

                m_lastLoggedOutcome = AIWorkOutcome.ChoseTask;
                controller.SetMoveTarget(currentTask.interactable.transform.position);
                controller.StateMachine.ChangeState(controller.MoveState);
                return;
            }
            skippedCount++;
        }

        if (m_lastLoggedOutcome != AIWorkOutcome.IdleAllClaimed)
        {
            ExperimentLogger.LogEvent("AI_DECISION", "all tasks claimed by others, going idle",
                "ai", controller.name,
                "mode", controller.TaskSchedulingMode.ToString(),
                "candidateCount", tasks.Count.ToString(),
                "skipped", skippedCount.ToString());
            m_lastLoggedOutcome = AIWorkOutcome.IdleAllClaimed;
        }
        controller.StateMachine.ChangeState(controller.IdleState);
    }

    private bool IsReferenceTaskValid(Task task)
    {
        if (task == null) return false;
        var manager = task.GetComponentInParent<TaskManager>();
        if (manager == null) return false;
        if (manager.Finished) return false;
        if (!manager.gameObject.activeInHierarchy) return false;
        return true;
    }

    public bool IsTaskValid(Task task)
    {
        if (task == null) return false;
        if (!task.gameObject.activeInHierarchy) return false;
        var manager = task.GetComponentInParent<TaskManager>();
        if (manager == null || manager.CompletableTasks == null || manager.Finished) return false;
        return manager.CompletableTasks.Contains(task);
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
        if (currentTask == null) return;

        if (!IsTaskValid(currentTask))
        {
            ExperimentLogger.LogEvent("AI_ABORT", "task became invalid before interact",
                "ai", controller.name,
                "task", currentTask.name,
                "interactable", currentTask.interactable.name);
            AbandonCurrentTask();
            return;
        }

        currentTask.interactable.ShowUI();
        if (!currentTask.interactable.StartInteraction(controller.m_interact))
        {
            ExperimentLogger.LogEvent("AI_ABORT", "interaction refused",
                "ai", controller.name,
                "task", currentTask.name,
                "interactable", currentTask.interactable.name);
            AbandonCurrentTask();
        }
    }

    public bool IsStepDone()
    {
        if (currentTask != null && currentTask.IsComplete)
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
            ExperimentLogger.LogEvent("AI_ABORT", "abandoning task",
                "ai", controller.name,
                "task", currentTask.name,
                "interactable", currentTask.interactable.name,
                "mode", controller.TaskSchedulingMode.ToString());

            if (currentTask.interactable.Interacter == controller.m_interact)
                currentTask.interactable.StopInteraction();

            AITaskCoordinator.ReleaseTask(currentTask, this);
            currentTask = null;
        }

        controller.StopMoving();
        RequestWork();
    }
}