
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject exitConfirmationPanel;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


public void ShowExitConfirmation()
{
    pauseMenuPanel.SetActive(false);
    exitConfirmationPanel.SetActive(true);

    Time.timeScale = 0f;
}

public void CancelExit()
{
    exitConfirmationPanel.SetActive(false);
    pauseMenuPanel.SetActive(true);

    Time.timeScale = 0f;
}

public void ConfirmExit()
{
    Time.timeScale = 1f;

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
}

public void RestartLevel()
{
    // Resume game time
    Time.timeScale = 1f;

    // Restart the current scene
    SceneManager.LoadScene(
        SceneManager.GetActiveScene().buildIndex
    );
}

public void GoToMainMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("main_menu");
}

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
