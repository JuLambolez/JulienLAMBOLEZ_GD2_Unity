using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string nextLevelSceneName;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private UIManager uiManager;
    private bool gameEnded = false;

    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        Time.timeScale = 1f;
    }

    public void LevelComplete()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowVictoryScreen();
        }
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowDeathScreen();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogWarning("Aucun niveau suivant défini !");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
