using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSeatTask : Task 
{
    public override void OnTaskEnd(Task task)
    {
        Transform parentSeat = interactable.transform.parent;
        for (int i = 0; i < parentSeat.childCount; i++)
        {
            Transform childSeat = parentSeat.GetChild(i);
            childSeat.localPosition = new Vector3(0, 0, 0); 
            childSeat.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
