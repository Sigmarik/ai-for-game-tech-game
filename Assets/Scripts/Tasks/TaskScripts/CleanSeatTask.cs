using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanSeatTask : Task
{
    public override void OnTaskEnd(Task task)
    {
        Transform parentSeat = interactable.transform.parent;
        foreach (Transform child in parentSeat.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "dirty_low_poly_car_seat") // This is real bad but works...
            {
                child.gameObject.SetActive(false);
                break;
            }
        }
    }
}
