using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorToggleTask : Task
{
    [SerializeField] public GameObject openedDoors;
    [SerializeField] public GameObject closedDoors;

    public override void OnTaskEnd(Task task)
    {
        closedDoors.SetActive(!closedDoors.active);
        openedDoors.SetActive(!openedDoors.active);
    }
}
