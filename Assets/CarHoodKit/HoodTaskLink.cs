using UnityEngine;

public class HoodTaskLink : MonoBehaviour
{
    [SerializeField] private HoodController hood;
    [SerializeField] private Task openHoodTask;
    [SerializeField] private Task closeHoodTask;

    private void OnEnable()
    {
        if (openHoodTask != null)
            openHoodTask.OnTaskComplete += OpenHood;

        if (closeHoodTask != null)
            closeHoodTask.OnTaskComplete += CloseHood;
    }

    private void OnDisable()
    {
        if (openHoodTask != null)
            openHoodTask.OnTaskComplete -= OpenHood;

        if (closeHoodTask != null)
            closeHoodTask.OnTaskComplete -= CloseHood;
    }

    private void OpenHood(Task completedTask)
    {
        if (hood != null)
            hood.OpenHood();
    }

    private void CloseHood(Task completedTask)
    {
        if (hood != null)
            hood.CloseHood();
    }
}