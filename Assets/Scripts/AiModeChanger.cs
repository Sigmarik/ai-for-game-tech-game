using UnityEngine;
using TMPro;

public class AiModeChanger : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown == null)
        {
            Debug.LogWarning($"{nameof(AiModeChanger)}: No TMP_Dropdown found on {gameObject.name}.", this);
            return;
        }

        // After a scene reload the dropdown defaults to index 0 and the
        // spawners/controllers default to their prefab values. Push the
        // persisted mode into the scene first.
        ApplyModeToWorld(AISettings.CurrentMode, reason: "initialized");

        // Reflect persisted mode in the UI WITHOUT firing onValueChanged
        // (which would re-run ApplyModeToWorld and log a spurious "changed").
        dropdown.SetValueWithoutNotify((int)AISettings.CurrentMode);

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDestroy()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        if (!System.Enum.IsDefined(typeof(AITaskSchedulingMode), index))
        {
            Debug.LogWarning($"AiModeChanger: dropdown index {index} does not map to an AITaskSchedulingMode.");
            return;
        }

        var mode = (AITaskSchedulingMode)index;
        AISettings.CurrentMode = mode;              // persist across scene reloads
        ApplyModeToWorld(mode, reason: "changed");
    }

    private void ApplyModeToWorld(AITaskSchedulingMode mode, string reason)
    {
        var controllers = Object.FindObjectsByType<AIPlayerController>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var c in controllers)
            c.TaskSchedulingMode = mode;

        var spawners = Object.FindObjectsByType<AISpawner>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var s in spawners)
            s.taskSchedulingModeOverride = mode;

        string label = dropdown != null && (int)mode < dropdown.options.Count
            ? dropdown.options[(int)mode].text
            : mode.ToString();

        ExperimentLogger.LogEvent("AI_MODE", reason,
            "mode", mode.ToString(),
            "index", ((int)mode).ToString(),
            "label", label,
            "controllersUpdated", controllers.Length.ToString(),
            "spawnersUpdated", spawners.Length.ToString());
    }
}