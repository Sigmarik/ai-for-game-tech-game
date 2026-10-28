using System.Collections.Generic;
using UnityEngine;

public static class TaskGraph
{
    public static Dictionary<Task, int> GetDistancesFrom(Task from)
    {
        var result = new Dictionary<Task, int>();
        if (from == null) return result;

        var allTasks = Object.FindObjectsByType<Task>(FindObjectsSortMode.None);
        var taskSet = new HashSet<Task>(allTasks);

        var adjacency = new Dictionary<Task, List<Task>>();
        foreach (var task in allTasks)
        {
            adjacency[task] = new List<Task>();
        }

        foreach (var task in allTasks)
        {
            foreach (var req in task.ReqTasks)
            {
                if (taskSet.Contains(req))
                {
                    adjacency[task].Add(req);
                    adjacency[req].Add(task);
                }
            }
        }

        var endTasks = new HashSet<Task>(allTasks);
        foreach (var task in allTasks)
        {
            foreach (var req in task.ReqTasks)
            {
                endTasks.Remove(req);
            }
        }

        var endTaskList = new List<Task>(endTasks);
        for (int i = 0; i < endTaskList.Count; i++)
        {
            for (int j = i + 1; j < endTaskList.Count; j++)
            {
                adjacency[endTaskList[i]].Add(endTaskList[j]);
                adjacency[endTaskList[j]].Add(endTaskList[i]);
            }
        }

        var queue = new Queue<(Task task, int distance)>();
        var visited = new HashSet<Task>();
        queue.Enqueue((from, 0));
        visited.Add(from);
        result[from] = 0;

        while (queue.Count > 0)
        {
            var (current, distance) = queue.Dequeue();

            if (adjacency.TryGetValue(current, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        result[neighbor] = distance + 1;
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }
        }

        TaskManager referenceManager = from.GetComponentInParent<TaskManager>();
        if (referenceManager != null)
        {
            var keys = new List<Task>(result.Keys);
            foreach (var task in keys)
            {
                TaskManager taskManager = task.GetComponentInParent<TaskManager>();
                if (taskManager != referenceManager)
                {
                    result[task] += 2;
                }
            }
        }

        return result;
    }
}