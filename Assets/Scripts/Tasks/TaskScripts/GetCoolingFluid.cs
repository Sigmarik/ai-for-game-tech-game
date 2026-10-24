using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFilterTask : Task 
{
    public void Start()
    {
        interactable = GameObject.Find("CoolingFluid").GetComponent<Interactable>();
    }
    public override void OnTaskEnd(Task task)
    {
        Destroy(interactable);
    }
}
