using UnityEngine;

// Collectible qui AJOUTE des points au score
// C'est un objet "bonus" que le joueur veut ramasser

public class Target_Soft : MonoBehaviour
{
    // Valeur en points de ce collectible (positif = bonus)
    [SerializeField] private int _targetValue = 5;

    // Référence à un effet de particules (optionnel, non utilisé actuellement)
    [SerializeField] private GameObject _particuleEffect;

    // OnPlayerCollect : Appelé quand le joueur collecte cet objet
    // 1. Récupère le composant Player_Collect du joueur
    // 2. Appelle UpdateScore() pour ajouter les points
    // 3. Détruit le collectible de la scène
    public void OnPlayerCollect(GameObject player)
    {
        // GetComponent cherche un composant sur le GameObject
        Player_Collect collector = player.GetComponent<Player_Collect>();

        if (collector != null)
        {
            // Ajoute les points au score
            collector.UpdateScore(_targetValue);

            // Destroy détruit l'objet de la scène
            Destroy(gameObject);
        }
    }
}
