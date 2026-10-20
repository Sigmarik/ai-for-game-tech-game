using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] public int spawnChannel = 1;
    [SerializeField] private GameObject aiPrefab;

    private GameObject visuals;

    public void SpawnAI()
    {
        Instantiate(aiPrefab, transform.position, transform.rotation);
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
