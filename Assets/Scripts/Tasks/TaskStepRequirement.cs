using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class TaskStepRequirement : ScriptableObject
{
    public virtual bool RequirementMet()
    {
        return true;
    }
}

[CreateAssetMenu(fileName = "NewItemHeldStepRequirement", menuName = "Tasks/Requirements/ItemHeld")]
public class ItemHeldStepRequirement : TaskStepRequirement
{
    public string reqItemTag;

    public override bool RequirementMet()
    {
        // TODO: Check if player that is initating step is holding the item.
        return true;
    }
}
