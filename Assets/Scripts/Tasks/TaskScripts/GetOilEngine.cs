using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOil : Task
{
    public void Start()
    {
        GameObject obj = GameObject.Find("OilEngine");
        interactable = obj.GetComponentInChildren<Interactable>();
    }

    public override void OnTaskEnd(Task task)
    {
        Destroy(interactable);
    }
}
