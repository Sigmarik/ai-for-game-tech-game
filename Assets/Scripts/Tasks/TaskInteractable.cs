using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Tire
}

public class TaskInteractable : MonoBehaviour
{
    [SerializeField]
    public InteractableType interactableType;
    private TaskInstance task;

    public void StartInteraction()
    {
        
    }

    public void StopInteraction()
    {

    }
}
