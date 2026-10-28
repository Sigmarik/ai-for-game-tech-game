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
        foreach (AIPlayerController ai in instantiated.GetComponentsInChildren<AIPlayerController>())
        {
            ai.TaskSchedulingMode = taskSchedulingModeOverride;
        }
    }

    public void SetParticlesActive(bool active)
    {
        if (visuals != null)
        {
            visuals.SetActive(active);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
        {
            visuals = transform.GetChild(0).gameObject;
            visuals.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
