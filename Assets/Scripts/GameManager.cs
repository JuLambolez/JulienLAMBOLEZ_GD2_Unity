using UnityEngine;
using UnityEngine.SceneManagement;

// Gère l'état global du jeu : victoire, défaite, changement de niveau
// - Détecter la fin du niveau (victoire ou défaite)
// - Afficher les bons écrans via UIManager
// - Charger les niveaux suivants ou le menu principal
// - Gérer le Time.timeScale (pause du jeu)

public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string nextLevelSceneName;      // Nom de la scène du niveau suivant
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Nom de la scène du menu principal
    [SerializeField] private bool isLastLevel = false;        // true si c'est le dernier niveau du jeu

    private UIManager uiManager; // Référence au gestionnaire d'interface
    private bool gameEnded = false; // Flag pour éviter de déclencher victoire/défaite plusieurs fois

    void Start()
    {
        // Trouve le UIManager dans la scène
        uiManager = FindFirstObjectByType<UIManager>();

        // S'assure que le temps est normal (pas en pause)
        // Time.timeScale = 1 : temps normal
        // Time.timeScale = 0 : pause totale
        Time.timeScale = 1f;
    }

    // LevelComplete : Appelé quand le joueur atteint le Goal
    // 1. Vérifie qu'on n'a pas déjà fini (évite les doublons)
    // 2. Met le jeu en pause
    // 3. Affiche l'écran de victoire approprié (normal ou final)
    public void LevelComplete()
    {
        // Protection contre les appels multiples
        if (gameEnded) return;

        gameEnded = true;      // Marque le jeu comme terminé
        Time.timeScale = 0f;   // Met le jeu en pause

        if (uiManager != null)
        {
            // Si c'est le dernier niveau : écran spécial avec score final
            if (isLastLevel)
            {
                int finalScore = GetFinalScore(); // Récupère le score depuis ScoreDatas
                uiManager.ShowGameCompleteScreen(finalScore);
            }
            // Sinon : écran de victoire normal avec bouton Next Level
            else
            {
                uiManager.ShowVictoryScreen();
            }
        }
    }

    // GameOver : Appelé quand le joueur n'a plus de coups
    public void GameOver()
    {
        // Protection contre les appels multiples
        if (gameEnded) return;

        gameEnded = true;      // Marque le jeu comme terminé
        Time.timeScale = 0f;   // Met le jeu en pause

        if (uiManager != null)
        {
            uiManager.ShowDeathScreen(); // Affiche l'écran Game Over
        }
    }
    // RestartLevel : Recharge le niveau actuel
    // Utilisé par le bouton "Retry" sur l'écran de défaite
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Remet le temps normal

        // SceneManager.GetActiveScene().name = nom de la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // LoadNextLevel : Charge le niveau suivant
    // Utilisé par le bouton "Next Level" sur l'écran de victoire
    public void LoadNextLevel()
    {
        Time.timeScale = 1f; // Remet le temps normal

        // Vérifie qu'un niveau suivant est défini
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName); // Charge la scène
        }
        else
        {
            Debug.LogWarning("Aucun niveau suivant défini !");
        }
    }

    // ReturnToMainMenu : Retourne au menu principal
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Remet le temps normal
        SceneManager.LoadScene(mainMenuSceneName); // Charge le menu principal
    }

    // GetFinalScore : Récupère le score final depuis ScoreDatas
    // Utilise Resources.FindObjectsOfTypeAll pour trouver le ScriptableObject
    // ScoreDatas dans le projet (même s'il n'est pas dans la scène)
    // Les ScriptableObjects ne sont pas dans la scène, mais dans les Assets
    // Resources.FindObjectsOfTypeAll permet de les trouver quand même
    private int GetFinalScore()
    {
        // Cherche le ScriptableObject ScoreDatas
        ScoreDatas scoreData = Resources.FindObjectsOfTypeAll<ScoreDatas>()[0];
        if (scoreData != null)
        {
            return scoreData.ScoreValue; // Retourne le score
        }
        return 0; // Score par défaut si non trouvé
    }
}
