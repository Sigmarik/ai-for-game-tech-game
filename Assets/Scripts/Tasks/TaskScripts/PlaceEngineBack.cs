using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceEngineBack : Task
{
    public override void OnTaskEnd(Task task)
    {
        interactable.transform.localPosition = new Vector3(-0.00153846166f, 0.23153846f, 1.13615382f);
        interactable.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
