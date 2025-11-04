using UnityEngine;
using TMPro;

// Gère tous les éléments d'interface utilisateur (UI) du jeu
// - Afficher/cacher les écrans (victoire, défaite, game complete, HUD)
// - Mettre à jour les textes dynamiques (score, coups restants)
// - S'abonner aux événements pour recevoir les notifications

public class UIManager : MonoBehaviour
{
    [Header("Écrans")]
    // Références aux différents panneaux UI (GameObject)
    [SerializeField] private GameObject victoryScreen;      // Écran de victoire (niveau normal)
    [SerializeField] private GameObject deathScreen;        // Écran de défaite (Game Over)
    [SerializeField] private GameObject gameCompleteScreen; // Écran de fin de jeu (dernier niveau)
    [SerializeField] private GameObject gameplayUI;         // HUD pendant le jeu (score, coups)

    [Header("Textes")]
    // Références aux textes qui changent pendant le jeu
    [SerializeField] private TextMeshProUGUI movesText;  // Texte affichant les coups restants
    [SerializeField] private TextMeshProUGUI scoreText;  // Texte affichant le score

    [Header("Textes Game Complete")]
    [SerializeField] private TextMeshProUGUI finalScoreText; // Texte du score final (écran fin de jeu)

    private MoveCounter moveCounter; // Référence au compteur de coups

    // 1. S'abonne aux événements du MoveCounter et Player_Collect
    // 2. Initialise l'affichage
    // 3. Cache tous les écrans sauf le HUD

    void Start()
    {
        // Trouve le MoveCounter dans la scène
        moveCounter = FindFirstObjectByType<MoveCounter>();

        // S'abonne à l'événement OnMovesChanged
        if (moveCounter != null)
        {
            // AddListener = "écoute cet événement et appelle cette fonction quand il se déclenche"
            moveCounter.OnMovesChanged.AddListener(UpdateMovesText);

            // Affiche la valeur initiale
            UpdateMovesText(moveCounter.MovesRemaining);
        }

        // S'abonne à l'événement statique OnTargetCollected
        // "+=" pour les Actions C# signifie "ajoute cette fonction aux écouteurs"
        Player_Collect.OnTargetCollected += UpdateScoreText;

        // Cache tous les écrans
        HideAllScreens();

        // Affiche uniquement le HUD de jeu
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }
    }

    // OnDestroy : Appelé quand cet objet est détruit
    // Si on ne le fait pas, le UIManager "fantôme" continuerait d'écouter
    // même après avoir changé de scène, ce qui causerait des erreurs
    void OnDestroy()
    {
        // "-=" pour se désabonner de l'événement
        Player_Collect.OnTargetCollected -= UpdateScoreText;
    }

    // UpdateMovesText : Met à jour l'affichage des coups restants
    // Cette fonction est appelée automatiquement par l'événement
    // OnMovesChanged du MoveCounter à chaque fois qu'un coup est utilisé
    private void UpdateMovesText(int movesRemaining)
    {
        if (movesText != null)
        {
            // $"..." est une "interpolated string" : on peut insérer des variables avec {variable}
            movesText.text = $"Coups restants: {movesRemaining}";
        }
    }

    // UpdateScoreText : Met à jour l'affichage du score
    // Cette fonction est appelée automatiquement par l'événement
    // OnTargetCollected de Player_Collect à chaque collectible ramassé
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    // ShowVictoryScreen : Affiche l'écran de victoire
    // Appelé par GameManager.LevelComplete() pour les niveaux normaux
    public void ShowVictoryScreen()
    {
        HideAllScreens(); // Cache tous les écrans d'abord

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true); // Affiche l'écran de victoire
        }
    }

    // ShowGameCompleteScreen : Affiche l'écran de fin de jeu
    // Appelé par GameManager.LevelComplete() pour le DERNIER niveau
    // Affiche un écran spécial avec le score final et un bouton Main Menu
    public void ShowGameCompleteScreen(int finalScore)
    {
        HideAllScreens(); // Cache tous les écrans d'abord

        if (gameCompleteScreen != null)
        {
            gameCompleteScreen.SetActive(true); // Affiche l'écran final

            // Met à jour le texte du score final
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score Final : {finalScore}";
            }
        }
    }

    // ShowDeathScreen : Affiche l'écran de Game Over
    // Appelé par GameManager.GameOver() quand le joueur n'a plus de coups
    public void ShowDeathScreen()
    {
        HideAllScreens(); // Cache tous les écrans d'abord

        if (deathScreen != null)
        {
            deathScreen.SetActive(true); // Affiche l'écran de défaite
        }
    }

    // HideAllScreens : Cache tous les écrans UI
    // On cache TOUS les écrans avant d'en afficher un nouveau
    // pour éviter d'avoir plusieurs écrans visibles en même temps
    private void HideAllScreens()
    {
        // SetActive(false) cache un GameObject
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (deathScreen != null) deathScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
    }
}
