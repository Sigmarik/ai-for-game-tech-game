using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task : MonoBehaviour
{
    [SerializeField] public Interactable interactable;
    [SerializeField] public GameObject returnItem;
    [SerializeField] public float interactDuration;

    private List<Task> reqTasks = new List<Task>();
    private bool isComplete = false;
    public bool IsComplete { get { return isComplete; } }
    private Coroutine activeTask;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            Task childTask = child.GetComponent<Task>();
            if (childTask != null) reqTasks.Add(childTask);
        }
        interactable.OnInteractionStart += Activate;
        interactable.OnInteractionEnd += Deactivate;
        interactable.TaskSubscribed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (isComplete || !reqTasks.All(task => task.IsComplete))
            return;
        activeTask = StartCoroutine(this.ActivateTask());
    }

    public void Deactivate()
    {
        if (activeTask != null)
        {
            StopCoroutine(activeTask);
            interactable.StopProgressBar();
            activeTask = null;
        }
    }

    public IEnumerator ActivateTask()
    {
        interactable.StartProgressBar(interactDuration);
        yield return new WaitForSeconds(interactDuration);
        interactable.StopProgressBar();
        interactable.NextState();
        interactable.OnInteractionStart -= Activate;
        interactable.OnInteractionEnd -= Deactivate;
        interactable.TaskUnsubscribed();
        isComplete = true;
    }
}
