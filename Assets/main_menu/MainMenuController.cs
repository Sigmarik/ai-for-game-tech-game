using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject exitConfirmationPanel;
    public GameObject mainMenu;

    public void OpenGarage()
    {
        SceneManager.LoadScene("Garage");
    }

    public void OpenPlayground()
    {
        SceneManager.LoadScene("LevelDirectorPlayground");
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("Garage");
    }

    public void ShowExitPanel()
    {
        exitConfirmationPanel.SetActive(true);
    }


public void CancelExit()
{
    exitConfirmationPanel.SetActive(false);
    mainMenu.SetActive(true);

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

}