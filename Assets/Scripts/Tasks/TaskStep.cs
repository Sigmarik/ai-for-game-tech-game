using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTaskStep", menuName = "Tasks/TaskStep")]
public class TaskStep : ScriptableObject 
{
    public List<TaskStepRequirement> requirements;
    public bool autonomous;
    public float duration;
    private bool isInitiated = false;
    public bool IsInitiated
    {
        get { return isInitiated;  }
        private set { isInitiated = value; }
    }

    public IEnumerator StartStep()
    {
        if (!autonomous)
        {
            // TODO: Turn off player that initated step movement
        }
        yield return new WaitForSeconds(duration);
        TaskManager.instance.StepComplete(this);
    }
}
