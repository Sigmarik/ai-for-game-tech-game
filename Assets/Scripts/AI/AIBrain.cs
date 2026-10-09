using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIBrain
{
    /// <summary>
    ///  Tell AI what gonna do next
    /// </summary>
    AIPlayerController controller;
    public AIBrain(AIPlayerController controller)
    {
        this.controller = controller;
    }

    public void RequestWork()
    {
        var target = controller.testTarget.position;
        controller.SetMoveTarget(target);
        controller.StateMachine.ChangeState(controller.MoveState);
    }

    public void OnMoveComplete()
    {
        controller.StateMachine.ChangeState(controller.InteractState);
    }

    public void OnInteractComplete()
    {
        RequestWork();
    }

    public void StartInteraction()
    {
        
    }

    public bool IsStepDone()
    {
        return true;
    }
}
