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
    [SerializeField] private TextMeshProUGUI scoreText;

    private MoveCounter moveCounter;

    void Start()
    {
        moveCounter = FindFirstObjectByType<MoveCounter>();

        if (moveCounter != null)
        {
            moveCounter.OnMovesChanged.AddListener(UpdateMovesText);
            UpdateMovesText(moveCounter.MovesRemaining);
        }

        Player_Collect.OnTargetCollected += UpdateScoreText;

        HideAllScreens();

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }
    }

    void OnDestroy()
    {
        Player_Collect.OnTargetCollected -= UpdateScoreText;
    }

    private void UpdateMovesText(int movesRemaining)
    {
        if (movesText != null)
        {
            movesText.text = $"Coups restants: {movesRemaining}";
        }
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
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
