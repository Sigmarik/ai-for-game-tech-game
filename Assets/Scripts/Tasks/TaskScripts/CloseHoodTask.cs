using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseHoodTask : Task
{
    [SerializeField] private HoodController controller;
    public override void OnTaskEnd(Task task)
    {
        controller.CloseHood();
    }
}
