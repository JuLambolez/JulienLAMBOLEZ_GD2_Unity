using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string LEVEL_ONE_SCENE_NAME = "Level1";

    public void StartGame()
    {
        SceneManager.LoadScene(LEVEL_ONE_SCENE_NAME, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
