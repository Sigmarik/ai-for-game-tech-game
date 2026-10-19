using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericLevelSequencer : MonoBehaviour
{
    [SerializeField] protected GameObject cameraRig;
    [SerializeField] protected List<GameObject> carPrefabs;
    [SerializeField] protected Transform carQueueOrigin;
    [SerializeField] protected Transform workshopPosition;
    [SerializeField] protected Transform exitPosition;
    [SerializeField] protected float distanceBetweenCars = 5f;

    [SerializeField] protected List<GameObject> confettiPrefabs;

    protected List<GameObject> m_carsToFix = new List<GameObject>();
    protected int m_currentCarIndex = 0;
    protected GameObject m_currentCar = null;

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
        for (int i = 0; i < m_carsToFix.Count; i++)
        {
            GameObject car = m_carsToFix[i];
            HighlightEveryInteractive(car, true);
            FocusCameraOn(car);
            yield return new WaitForSeconds(2f);
            HighlightEveryInteractive(car, false);
        }
        ResetCameraFocus();
    }

    protected IEnumerator RemoveCurrentCar()
    {
        if (m_currentCar != null)
        {
            DisableAllColliders(m_currentCar);
            StartCoroutine(LerpObjectToPosition(m_currentCar, exitPosition.position, 2f));
            m_currentCar = null;
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected IEnumerator AdvanceQueue()
    {
        if (m_currentCarIndex < m_carsToFix.Count)
        {
            m_currentCar = m_carsToFix[m_currentCarIndex];
            StartCoroutine(MoveAndActivateCar(m_currentCar, workshopPosition.position, 1f));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(MoveCarsInQueue());
        }
        m_currentCarIndex++;
    }

    private IEnumerator MoveAndActivateCar(GameObject car, Vector3 targetPosition, float duration)
    {
        yield return LerpObjectToPosition(car, targetPosition, duration);
        MakeTaskManagerActive(car);
    }

    protected IEnumerator MoveCarsInQueue()
    {
        int nextCarIndex = m_currentCarIndex + 1;
        for (int i = nextCarIndex; i < m_carsToFix.Count; i++)
        {
            GameObject car = m_carsToFix[i];
            Vector3 targetPosition = carQueueOrigin.position - carQueueOrigin.forward * distanceBetweenCars * (i - nextCarIndex + 0.5f);
            StartCoroutine(LerpObjectToPosition(car, targetPosition, 1f));
            yield return new WaitForSeconds(0.2f);
        }
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
            interactable.gameObject.SetHighlight(highlight);
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

        bool prevKeyState = Input.GetKeyDown(keyName);

        float elapsed = 0f;
        while (elapsed < promptDelay)
        {
            bool keyState = Input.GetKeyDown(keyName);
            if (keyState && !prevKeyState)
            {
                ShowMessage("ActionPrompt", "", false);
                yield break;
            }
            prevKeyState = keyState;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        ShowMessage("ActionPrompt", $"Press {keyName} to continue...", true);

        yield return new WaitUntil(() => Input.GetKeyDown(keyName));

        ShowMessage("ActionPrompt", "", false);
    }

    private float EaseInOut(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    protected IEnumerator LerpObjectToPosition(GameObject gameObject, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = gameObject.transform.position;
        Vector3 overallDirection = (targetPosition - startPosition).normalized; // fallback
        float elapsedTime = 0f;
        Vector3 previousPosition = startPosition;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float easedT = EaseInOut(t);

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, easedT);
            gameObject.transform.position = newPosition;

            Vector3 moveDir = newPosition - previousPosition;
            if (moveDir.sqrMagnitude > 1e-8f)
            {
                Vector3 flatDir = moveDir;

                if (flatDir.sqrMagnitude > 1e-8f)
                {
                    gameObject.transform.rotation =
                        Quaternion.LookRotation(flatDir.normalized, Vector3.up);
                }
            }

            previousPosition = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = targetPosition;

        if (overallDirection.sqrMagnitude > 1e-8f)
        {
            Vector3 flatDir = overallDirection;
            if (flatDir.sqrMagnitude > 1e-8f)
            {
                gameObject.transform.rotation =
                    Quaternion.LookRotation(flatDir.normalized, Vector3.up);
            }
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
        for (int i = 0; i < carPrefabs.Count; i++)
        {
            GameObject carPrefab = carPrefabs[i];
            Vector3 position = carQueueOrigin.position - carQueueOrigin.forward * distanceBetweenCars * (i + 0.5f);
            GameObject car = Instantiate(carPrefab, position, carQueueOrigin.rotation);
            m_carsToFix.Add(car);
        }
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
