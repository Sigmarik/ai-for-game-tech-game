using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveEngine : Task
{
    [SerializeField] private GameObject engine;
    public override void OnTaskEnd(Task task)
    {
        engine.transform.localPosition = new Vector3(2.44400001f, -0.32100001f, 2.12400007f);
        engine.transform.localRotation = Quaternion.Euler(0, -90, 0);
        interactable.SetText("Add oil and cooling fluid and put back.");
    }
}
