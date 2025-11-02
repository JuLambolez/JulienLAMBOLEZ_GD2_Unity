using UnityEngine;
using UnityEngine.Events;

public class MoveCounter : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int maxMoves = 30;

    [Header("Événements")]
    public UnityEvent<int> OnMovesChanged;
    public UnityEvent OnNoMovesRemaining;

    private int movesRemaining;

    public int MovesRemaining => movesRemaining;
    public int MaxMoves => maxMoves;

    void Start()
    {
        ResetMoves();
    }

    public void UseMove(bool isWinningMove = false)
    {
        if (movesRemaining > 0)
        {
            movesRemaining--;
            OnMovesChanged?.Invoke(movesRemaining);

            if (movesRemaining <= 0 && !isWinningMove)
            {
                OnNoMovesRemaining?.Invoke();
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
            }
        }
    }

    public void ResetMoves()
    {
        movesRemaining = maxMoves;
        OnMovesChanged?.Invoke(movesRemaining);
    }

    public void SetMaxMoves(int moves)
    {
        maxMoves = moves;
        ResetMoves();
    }
}
