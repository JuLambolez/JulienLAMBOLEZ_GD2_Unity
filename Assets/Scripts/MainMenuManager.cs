using UnityEngine;
using UnityEngine.SceneManagement;


// Gère les interactions du menu principal du jeu
// - Démarrer une nouvelle partie (charger Level1)
// - Quitter le jeu
// Ce script est attaché à un GameObject dans la scène MainMenu
// Les boutons UI appellent les fonctions publiques de ce script
// Ex : Bouton "Play" ? appelle StartGame()
//      Bouton "Quit" ? appelle QuitGame()

public class MainMenuManager : MonoBehaviour
{
    // CONSTANTE : nom de la première scène à charger
    // "const" signifie que cette valeur ne changera JAMAIS pendant l'exécution
    // En MAJUSCULES par convention pour les constantes
    // - Si on change le nom de la scène Level1, on le change à UN SEUL endroit
    // - Évite les fautes de frappe ("Leve1" au lieu de "Level1")
    // - Le compilateur peut optimiser le code
    private const string LEVEL_ONE_SCENE_NAME = "Level1";

    // StartGame : Lance une nouvelle partie
    // Charge la scène Level1 pour commencer le jeu
    public void StartGame()
    {
        // SceneManager.LoadScene charge une nouvelle scène
        // - LEVEL_ONE_SCENE_NAME : nom de la scène à charger
        // - LoadSceneMode.Single : remplace complètement la scène actuelle
        SceneManager.LoadScene(LEVEL_ONE_SCENE_NAME, LoadSceneMode.Single);
    }

    // QuitGame : Quitte l'application
    // Ferme complètement le jeu
    public void QuitGame()
    {
        // Quitte l'application
        Application.Quit();
#if UNITY_EDITOR

        // Arrête le mode Play dans l'éditeur Unity
        UnityEditor.EditorApplication.isPlaying = false;

#endif
    }
}
