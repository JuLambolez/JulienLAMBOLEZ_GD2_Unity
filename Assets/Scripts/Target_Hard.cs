using UnityEngine;

// Collectible qui RETIRE des points au score
// C'est un objet "piège" que le joueur veut ÉVITER

public class Target_Hard : MonoBehaviour
{
    // Valeur en points de ce collectible (négatif = malus)
    [SerializeField] private int _targetValue = -3;

    // FONCTIONNEMENT IDENTIQUE à Target_Soft
    // La seule différence est la valeur (_targetValue est négatif)
    public void OnPlayerCollect(GameObject player)
    {
        // GetComponent cherche le composant Player_Collect sur le joueur
        Player_Collect collector = player.GetComponent<Player_Collect>();

        if (collector != null)
        {
            // Retire des points au score (valeur négative)
            collector.UpdateScore(_targetValue);

            // Détruit l'objet de la scène
            Destroy(gameObject);
        }
    }
}
