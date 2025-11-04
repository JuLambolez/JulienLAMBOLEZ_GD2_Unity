using UnityEngine;
using UnityEngine.Events;


// Gère le nombre de coups restants au joueur
// Système de limite de mouvements : le joueur a un nombre maximum de déplacements
// pour terminer le niveau, sinon c'est Game Over
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

    // - isWinningMove : true si ce coup permet d'atteindre le Goal (pour éviter le Game Over sur un coup gagnant)
    // - deferGameOver : true pour reporter le Game Over après la fin du mouvement (pour éviter d'afficher Game Over avant que le joueur bouge)
    public void UseMove(bool isWinningMove = false, bool deferGameOver = false)
    {
        if (movesRemaining > 0)
        {
            movesRemaining--;
            OnMovesChanged?.Invoke(movesRemaining); // Notifie que le nombre de coups a changé

            // Si plus de coups ET que ce n'est pas un coup gagnant ET qu'on ne reporte pas le Game Over
            if (movesRemaining <= 0 && !isWinningMove && !deferGameOver)
            {
                OnNoMovesRemaining?.Invoke(); // Notifie qu'il n'y a plus de coups

                // Cherche le GameManager dans la scène
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.GameOver(); // Déclenche le Game Over
                }
            }
        }
    }
    public void ResetMoves()
    {
        movesRemaining = maxMoves; // Remet le compteur au max
        OnMovesChanged?.Invoke(movesRemaining); // Notifie le changement
    }
    public void SetMaxMoves(int moves)
    {
        maxMoves = moves;
        ResetMoves(); // Réinitialise avec la nouvelle valeur
    }
}
