using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static bool AutoStartNextLoad = false;

    public HoleSystemController holeController;
    public Conditions conditions;

    public GameObject gameplayUI;
    public GameObject mainMenuPanel;
    public GameObject winPanel;

    private void Start()
    {
        Time.timeScale = 1f;

        if (AutoStartNextLoad)
        {
            AutoStartNextLoad = false;
            StartGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        if (holeController != null)
        {
            holeController.CanMove = true;
        }

        if (conditions != null)
        {
            conditions.ShowHud = true;
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;

        if (holeController != null)
        {
            holeController.CanMove = false;
        }

        if (conditions != null)
        {
            conditions.ShowHud = false;
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void ShowWinPanel()
    {
        Time.timeScale = 0f;

        if (holeController != null)
        {
            holeController.CanMove = false;
        }

        if (conditions != null)
        {
            conditions.ShowHud = false;
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        AutoStartNextLoad = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        AutoStartNextLoad = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}