using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceWheelTask : Task
{
    public override void OnTaskEnd(Task task)
    {
        interactable.GetComponent<MeshRenderer>().enabled = true;
    }
}
