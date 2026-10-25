using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveEngine : Task
{
    public override void OnTaskEnd(Task task)
    {
        interactable.transform.localPosition = new Vector3(1.44f, 0.28f, 2.3f);
        interactable.transform.localRotation = Quaternion.Euler(0, -90, 0);
    }
}
