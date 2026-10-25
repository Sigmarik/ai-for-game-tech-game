using System.Collections;
using UnityEngine;

public class HoodTaskStarter : MonoBehaviour
{
    [SerializeField] private TaskManager taskManager;

    private IEnumerator Start()
    {
        // Allow Interactable.Start() to create its outline first.
        yield return null;

        if (taskManager == null)
        {
            Debug.LogError("Assign TaskManager in HoodTaskStarter.", this);
            yield break;
        }

        taskManager.MakeActive();
    }
}