using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSeatTask : Task
{
    public override void OnTaskEnd(Task task)
    {
        Transform parentSeat = interactable.transform.parent;
        for (int i = 0; i < parentSeat.childCount; i++)
        {
            Transform childSeat = parentSeat.GetChild(i);
            childSeat.localPosition = new Vector3(-7f, 0, 7f); 
            childSeat.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
