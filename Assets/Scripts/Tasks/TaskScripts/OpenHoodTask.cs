using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHoodTask : Task
{
    [SerializeField] private HoodController controller;

    public override void OnTaskEnd(Task task)
    {
        controller.OpenHood();
    }
}
