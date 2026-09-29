using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "Tasks/Task")]
public class TaskDefinition : ScriptableObject
{
    public string taskName;
    public List<TaskStepDefinition> taskSteps = new List<TaskStepDefinition>();
}
