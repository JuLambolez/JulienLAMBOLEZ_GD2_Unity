using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Manager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadANewLevel(buildIndex: 1);
        }
    }

    public void LoadANewLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
