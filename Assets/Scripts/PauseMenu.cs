using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isPaused;

    void Awake()
    {
        // Unity will auto-add this component because of [RequireComponent]
        canvasGroup = GetComponent<CanvasGroup>();

        // Start hidden
        SetVisible(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) UnpauseGame();
            else          PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetVisible(true);

        ExperimentLogger.LogEvent("PAUSE", "game paused",
            "timeScale", Time.timeScale.ToString("F2"),
            "unscaledTime", Time.unscaledTime.ToString("F3"));
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetVisible(false);

        ExperimentLogger.LogEvent("PAUSE", "game unpaused",
            "timeScale", Time.timeScale.ToString("F2"),
            "unscaledTime", Time.unscaledTime.ToString("F3"));
    }

    public void ResetGame()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Log BEFORE LoadScene — the scene (and anything scene-scoped) is torn
        // down during the load, so this is the last safe point to emit an event.
        ExperimentLogger.LogEvent("PAUSE", "game restarted",
            "scene", sceneName,
            "wasPaused", isPaused.ToString(),
            "timeScaleBefore", Time.timeScale.ToString("F2"),
            "unscaledTime", Time.unscaledTime.ToString("F3"));

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    void SetVisible(bool visible)
    {
        canvasGroup.alpha          = visible ? 1f : 0f;
        canvasGroup.interactable   = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}