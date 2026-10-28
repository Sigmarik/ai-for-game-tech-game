public static class PlayerTaskTracker
{
    private static Task lastPlayerTask;

    public static Task LastPlayerTask
    {
        get => lastPlayerTask;
        set
        {
            if (lastPlayerTask == value) return;
            Task previousTask = lastPlayerTask;
            lastPlayerTask = value;

            if (value != null)
            {
                ExperimentLogger.LogEvent("PLAYER_TASK", "system set current player task",
                    "task", value.name,
                    "interactable", value.interactable != null ? value.interactable.name : "none",
                    "previous", previousTask != null ? previousTask.name : "none");
            }
            else if (previousTask != null)
            {
                ExperimentLogger.LogEvent("PLAYER_TASK", "player task cleared",
                    "previous", previousTask.name);
            }
        }
    }
}