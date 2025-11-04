using System;
using UnityEngine;

// Gère le système de score du joueur
// 1. Les collectibles appellent UpdateScore() avec une valeur (+5, -3, etc.)
// 2. Le score est enregistré dans le ScriptableObject ScoreDatas
// 3. Un événement est envoyé pour notifier l'UI

public class Player_Collect : MonoBehaviour
{
    // Référence au ScriptableObject qui stocke le score
    // [SerializeField] permet de l'assigner dans l'Inspector Unity
    [SerializeField] private ScoreDatas _scoreData;

    // Événement statique : TOUTES les instances de Player_Collect partagent cet événement
    // Action<int> = événement qui transporte une valeur int (le score)
    // static = existe au niveau de la classe, pas de l'instance
    public static Action<int> OnTargetCollected;

    // UpdateScore : Modifie le score du joueur
    // 1. Ajoute la valeur au score dans ScoreDatas
    // 2. Limite le score minimum à -100 (pour éviter des scores trop négatifs)
    // 3. Déclenche l'événement OnTargetCollected pour notifier l'UI
    // Target_Soft appelle UpdateScore(5) ? score augmente
    // Target_Hard appelle UpdateScore(-3) ? score diminue
    public void UpdateScore(int value)
    {
        // Ajoute la valeur au score actuel
        _scoreData.ScoreValue += value;

        // Mathf.Max retourne la plus grande des deux valeurs
        // Ça empêche le score de descendre en dessous de -100
        _scoreData.ScoreValue = Mathf.Max(_scoreData.ScoreValue, -100);

        // Le "?." signifie "si l'événement a des écouteurs, appelle-le"
        // Invoke déclenche l'événement et envoie le nouveau score
        OnTargetCollected?.Invoke(_scoreData.ScoreValue);
    }
}
