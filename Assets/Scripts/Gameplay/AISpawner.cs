using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] public int spawnChannel = 1;
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private AITaskSchedulingMode taskSchedulingModeOverride = AITaskSchedulingMode.Closest;

    private GameObject visuals;

    public void SpawnAI()
    {
        GameObject instantiated = Instantiate(aiPrefab, transform.position, transform.rotation);

        string aiKind = taskSchedulingModeOverride == AITaskSchedulingMode.Closest ? "close" : "distant";

        ExperimentLogger.LogEvent("AI_SPAWN", "spawning AI",
            "prefab", aiPrefab.name,
            "kind", aiKind,
            "schedulingMode", taskSchedulingModeOverride.ToString(),
            "channel", spawnChannel.ToString(),
            "position", transform.position.ToString("F2"));

        foreach (AIPlayerController ai in instantiated.GetComponentsInChildren<AIPlayerController>())
        {
            ai.TaskSchedulingMode = taskSchedulingModeOverride;

            ExperimentLogger.LogEvent("AI_SPAWN", "AI controller configured",
                "name", ai.gameObject.name,
                "kind", aiKind,
                "schedulingMode", taskSchedulingModeOverride.ToString());
        }
    }

    public void SetParticlesActive(bool active)
    {
        if (visuals != null)
        {
            visuals.SetActive(active);
        }
    }

    void Start()
    {
        if (transform.childCount > 0)
        {
            visuals = transform.GetChild(0).gameObject;
            visuals.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}