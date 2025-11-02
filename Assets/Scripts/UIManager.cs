using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Écrans")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject gameplayUI;

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI movesText;

    private MoveCounter moveCounter;

    void Start()
    {
        moveCounter = FindFirstObjectByType<MoveCounter>();

        if (moveCounter != null)
        {
            moveCounter.OnMovesChanged.AddListener(UpdateMovesText);
            UpdateMovesText(moveCounter.MovesRemaining);
        }

        HideAllScreens();

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }
    }

    private void UpdateMovesText(int movesRemaining)
    {
        if (movesText != null)
        {
            movesText.text = $"Remaining Moves: {movesRemaining}";
        }
    }

    public void ShowVictoryScreen()
    {
        HideAllScreens();
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }
    }

    public void ShowDeathScreen()
    {
        HideAllScreens();
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }

    private void HideAllScreens()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (deathScreen != null) deathScreen.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
    }
}
