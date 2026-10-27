using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class FixDent : Task
{
    [SerializeField] private GameObject brokenModel;
    [SerializeField] private GameObject repairedModel;

    public override void OnTaskEnd(Task task)
    {
        SetRepaired();
    }
    public void SetRepaired()
    {
        brokenModel.SetActive(false);

        repairedModel.SetActive(true);
    }
    
}
