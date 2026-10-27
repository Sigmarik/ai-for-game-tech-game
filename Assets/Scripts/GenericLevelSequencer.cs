using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericLevelSequencer : MonoBehaviour
{
    [SerializeField] protected GameObject cameraRig;
    [SerializeField] protected List<GameObject> carPrefabs;
    [SerializeField] protected Transform carQueueOrigin;
    [SerializeField] protected Transform workshopPosition;
    [SerializeField] protected Transform exitGatePosition;
    [SerializeField] protected Transform exitPosition;
    [SerializeField] protected float distanceBetweenCars = 5f;
    [SerializeField] protected float workshopSpotSeparation = 3f;
    [SerializeField] protected int maxCarsInWorkshop = 2;

    [SerializeField] protected List<GameObject> confettiPrefabs;

    protected List<GameObject> m_carsToFix = new List<GameObject>();
    protected List<GameObject> m_activeCars = new List<GameObject>();
    protected int m_currentCarIndex = 0;
    protected GameObject m_currentCar = null;

    protected Vector3 WorkshopForwardDirection => (workshopPosition.position - carQueueOrigin.position).normalized;
    protected Vector3 WorkshopRightDirection => Vector3.Cross(Vector3.up, WorkshopForwardDirection).normalized;

    abstract protected IEnumerator LevelSequence();

    protected List<AISpawner> GetSpawnersAtChannel(int channel)
    {
        List<AISpawner> spawnersAtChannel = new List<AISpawner>();
        AISpawner[] spawners = FindObjectsByType<AISpawner>(FindObjectsSortMode.None);
        foreach (AISpawner spawner in spawners)
        {
            if (spawner.spawnChannel == channel)
            {
                spawnersAtChannel.Add(spawner);
            }
        }
        return spawnersAtChannel;
    }

    protected IEnumerator PreviewCars()
    {
        for (int carIndex = 0; carIndex < m_carsToFix.Count; carIndex++)
        {
            GameObject car = m_carsToFix[carIndex];
            HighlightEveryInteractive(car, true);
            FocusCameraOn(car);
            yield return new WaitForSeconds(2f);
            HighlightEveryInteractive(car, false);
        }
        ResetCameraFocus();
    }

    protected IEnumerator AdvanceQueueToWorkshop()
    {
        int availableSpotCount = maxCarsInWorkshop - m_activeCars.Count;
        int carsToAdvance = Mathf.Min(availableSpotCount, m_carsToFix.Count - m_currentCarIndex);

        for (int carOffset = 0; carOffset < carsToAdvance; carOffset++)
        {
            GameObject car = m_carsToFix[m_currentCarIndex];
            int spotIndex = m_activeCars.Count;
            m_activeCars.Add(car);
            m_currentCar = m_activeCars[0];
            m_currentCarIndex++;
            yield return MoveAndActivateCar(car, GetWorkshopSpotPosition(spotIndex), 1f);
        }

        StartCoroutine(MoveCarsInQueue());
    }

    protected Vector3 GetWorkshopSpotPosition(int spotIndex)
    {
        float offset = (spotIndex == 0) ? -workshopSpotSeparation * 0.5f : workshopSpotSeparation * 0.5f;
        return workshopPosition.position + WorkshopRightDirection * offset;
    }

    protected IEnumerator MoveAndActivateCar(GameObject car, Vector3 targetPosition, float duration)
    {
        Quaternion targetRotation = Quaternion.LookRotation(WorkshopForwardDirection, Vector3.up);
        Vector3 startPosition = car.transform.position;
        Vector3 controlPoint = startPosition + (targetPosition - startPosition) * 0.5f + WorkshopForwardDirection * 2f;
        yield return LerpObjectToPosition(car, targetPosition, duration, targetRotation, controlPoint);
        MakeTaskManagerActive(car);
    }

    protected IEnumerator MoveCarsInQueue()
    {
        for (int queueIndex = m_currentCarIndex; queueIndex < m_carsToFix.Count; queueIndex++)
        {
            GameObject car = m_carsToFix[queueIndex];
            int queuePositionIndex = queueIndex - m_currentCarIndex;
            Vector3 targetPosition = carQueueOrigin.position - carQueueOrigin.forward * distanceBetweenCars * (queuePositionIndex + 0.5f);
            StartCoroutine(LerpObjectToPosition(car, targetPosition, 1f));
            yield return new WaitForSeconds(0.2f);
        }
    }

    protected bool AreAllActiveCarsFixed()
    {
        if (m_activeCars.Count == 0)
            return false;
        foreach (GameObject car in m_activeCars)
        {
            if (!IsCarFixed(car))
                return false;
        }
        return true;
    }

    protected IEnumerator RemoveAllActiveCars()
    {
        List<GameObject> carsToRemove = new List<GameObject>(m_activeCars);
        m_activeCars.Clear();
        m_currentCar = null;

        List<Coroutine> exitCoroutines = new List<Coroutine>();
        foreach (GameObject car in carsToRemove)
        {
            exitCoroutines.Add(StartCoroutine(RemoveCarFromWorkshop(car)));
            yield return new WaitForSeconds(1f);
        }

        foreach (Coroutine exitCoroutine in exitCoroutines)
        {
            yield return exitCoroutine;
        }
    }

    protected IEnumerator RemoveCarFromWorkshop(GameObject car)
    {
        if (car == null)
            yield break;
        DisableAllColliders(car);
        
        Vector3 startPosition = car.transform.position;
        Vector3 gatePosition = exitGatePosition.position;
        Quaternion gateRotation = exitGatePosition.rotation;
        Vector3 controlPoint = startPosition + (gatePosition - startPosition) * 0.5f;// + WorkshopForwardDirection * 2f;

        yield return LerpObjectToPosition(car, gatePosition, 1f, gateRotation, controlPoint);
        yield return LerpObjectToPosition(car, exitPosition.position, 2f, exitPosition.rotation);
    }

    protected IEnumerator SpawnConfetti()
    {
        if (confettiPrefabs.Count == 0)
        {
            Debug.LogWarning("No confetti prefabs assigned to LevelSequencer.");
            yield break;
        }

        Camera mainCamera = Camera.main;

        while (true)
        {
            GameObject confettiPrefab = confettiPrefabs[Random.Range(0, confettiPrefabs.Count)];
            if (confettiPrefab != null)
            {
                Vector3 spawnPosition = workshopPosition.position
                    + new Vector3(Random.Range(-2f, 2f), Random.Range(0.3f, 2.5f), Random.Range(-2f, 2f));

                Quaternion spawnRotation = Quaternion.identity;
                if (mainCamera != null)
                {
                    Vector3 toCamera = mainCamera.transform.position - spawnPosition;
                    if (toCamera.sqrMagnitude > 0.0001f)
                        spawnRotation = Quaternion.LookRotation(toCamera);
                }

                GameObject confetti = Instantiate(confettiPrefab, spawnPosition, spawnRotation);
                confetti.transform.localScale = Vector3.one * Random.Range(0.1f, 0.15f);
                Destroy(confetti, 5f);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    protected static void SetFreezeControls(bool freeze)
    {
        GenericController[] controllers = FindObjectsByType<GenericController>(FindObjectsSortMode.None);

        foreach (GenericController controller in controllers)
        {
            controller.controlsFrozen = freeze;
        }
    }

    protected void HighlightEveryInteractive(GameObject car, bool highlight)
    {
        foreach (Interactable interactable in car.GetComponentsInChildren<Interactable>(true))
        {
            if (highlight)
                interactable.SetActive(true);
            interactable.SetHighlight(highlight);
            if (!highlight)
                interactable.SetActive(false);
        }
    }

    protected void DisableAllColliders(GameObject car)
    {
        foreach (Collider collider in car.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
        }
    }

    protected void MakeTaskManagerActive(GameObject car)
    {
        foreach (TaskManager manager in car.GetComponentsInChildren<TaskManager>(true))
        {
            manager.MakeActive();
        }
    }

    protected bool IsCarFixed(GameObject car)
    {
        foreach (TaskManager manager in car.GetComponentsInChildren<TaskManager>(true))
        {
            if (!manager.Finished)
                return false;
        }
        return true;
    }

    protected IEnumerator WaitForInput(string keyName, float promptDelay = 3f)
    {
        Debug.Log($"Waiting for input: {keyName}");

        bool previousKeyState = Input.GetKeyDown(keyName);

        float elapsedTime = 0f;
        while (elapsedTime < promptDelay)
        {
            bool currentKeyState = Input.GetKeyDown(keyName);
            if (currentKeyState && !previousKeyState)
            {
                ShowMessage("ActionPrompt", "", false);
                yield break;
            }
            previousKeyState = currentKeyState;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        ShowMessage("ActionPrompt", $"Press {keyName} to continue...", true);

        yield return new WaitUntil(() => Input.GetKeyDown(keyName));

        ShowMessage("ActionPrompt", "", false);
    }

    private float EaseInOut(float progress)
    {
        return progress * progress * progress * (progress * (progress * 6f - 15f) + 10f);
    }

    protected IEnumerator LerpObjectToPosition(GameObject gameObject, Vector3 targetPosition, float duration, Quaternion? finalRotation = null, Vector3? controlPoint = null)
    {
        Vector3 startPosition = gameObject.transform.position;
        Quaternion startRotation = gameObject.transform.rotation;
        float elapsedTime = 0f;
        Vector3 previousPosition = startPosition;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            float easedProgress = EaseInOut(progress);

            Vector3 newPosition;
            if (controlPoint.HasValue)
            {
                float oneMinusProgress = 1f - easedProgress;
                newPosition = oneMinusProgress * oneMinusProgress * startPosition + 2f * oneMinusProgress * easedProgress * controlPoint.Value + easedProgress * easedProgress * targetPosition;
            }
            else
            {
                newPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            }

            gameObject.transform.position = newPosition;

            Vector3 moveDirection = newPosition - previousPosition;
            if (moveDirection.sqrMagnitude > 1e-8f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                if (finalRotation.HasValue)
                {
                    float rotationBlend = Mathf.Clamp01((progress - 0.7f) / 0.3f);
                    gameObject.transform.rotation = Quaternion.Slerp(lookRotation, finalRotation.Value, rotationBlend);
                }
                else
                {
                    gameObject.transform.rotation = lookRotation;
                }
            }

            previousPosition = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = targetPosition;

        if (finalRotation.HasValue)
        {
            gameObject.transform.rotation = finalRotation.Value;
        }
    }

    protected void ShowMessage(string channel, string message, bool pulse = false)
    {
        MessageBox[] messageBoxes = FindObjectsByType<MessageBox>(FindObjectsSortMode.None);

        foreach (MessageBox messageBox in messageBoxes)
        {
            if (messageBox.channel == channel)
            {
                messageBox.DisplayMessage(message, pulse);
            }
        }
    }

    private Transform m_playerTransform = null;

    protected void FocusCameraOn(GameObject target)
    {
        if (cameraRig != null && target != null)
        {
            Cinemachine.CinemachineVirtualCamera virtualCamera = cameraRig.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            if (m_playerTransform == null)
            {
                m_playerTransform = virtualCamera.Follow;
            }
            virtualCamera.Follow = target.transform;

            Cinemachine.CinemachineConfiner confiner = cameraRig.GetComponentInChildren<Cinemachine.CinemachineConfiner>();
            if (confiner != null)
            {
                confiner.enabled = false;
            }
        }
    }

    protected void ResetCameraFocus()
    {
        if (cameraRig != null && m_playerTransform != null)
        {
            Cinemachine.CinemachineVirtualCamera virtualCamera = cameraRig.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            virtualCamera.Follow = m_playerTransform;

            Cinemachine.CinemachineConfiner confiner = cameraRig.GetComponentInChildren<Cinemachine.CinemachineConfiner>();
            if (confiner != null)
            {
                confiner.enabled = true;
            }
        }
    }

    private void SpawnCarsInQueue()
    {
        for (int carIndex = 0; carIndex < carPrefabs.Count; carIndex++)
        {
            GameObject carPrefab = carPrefabs[carIndex];
            Vector3 position = carQueueOrigin.position - carQueueOrigin.forward * distanceBetweenCars * (carIndex + 0.5f);
            GameObject car = Instantiate(carPrefab, position, carQueueOrigin.rotation);
            m_carsToFix.Add(car);
        }
    }

    void Start()
    {
        if (cameraRig == null)
        {
            Cinemachine.CinemachineVirtualCamera virtualCamera = FindFirstObjectByType<Cinemachine.CinemachineVirtualCamera>();
            if (virtualCamera != null)
                cameraRig = virtualCamera.gameObject;
        }

        SpawnCarsInQueue();

        StartCoroutine(LevelSequence());
    }

    void Update()
    {

    }
}