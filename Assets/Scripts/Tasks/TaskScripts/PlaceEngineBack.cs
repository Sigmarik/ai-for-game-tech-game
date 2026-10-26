using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceEngineBack : Task
{
    [SerializeField] private GameObject engine;
    public override void OnTaskEnd(Task task)
    {
        engine.transform.localPosition = Vector3.zero;
        engine.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
