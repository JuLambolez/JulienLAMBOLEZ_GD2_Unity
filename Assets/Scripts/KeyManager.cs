using UnityEngine;
using System;

// Gère le système de clés : compte combien le joueur en a collecté
// et notifie les portes quand toutes les clés ont été ramassées
// 1. Écoute l'événement Target_Key.OnKeyCollected
// 2. Incrémente le compteur de clés à chaque collecte
// 3. Déclenche son propre événement OnKeysCollected
// 4. Les KeyDoor écoutent cet événement pour savoir quand s'ouvrir

public class KeyManager : MonoBehaviour
{
    [Header("Configuration")]
    // Nombre total de clés nécessaires pour ouvrir les portes
    [SerializeField] private int totalKeysRequired = 3;

    // Événement déclenché à chaque fois qu'une clé est collectée
    // Action<int> = événement qui transporte le nombre de clés collectées
    public Action<int> OnKeysCollected;

    private int keysCollected = 0; // Compteur de clés collectées
    public int KeysCollected => keysCollected;
    public int TotalKeysRequired => totalKeysRequired;


    // OnEnable est appelé AVANT Start, donc on s'assure d'être abonné
    // à l'événement dès que possible pour ne rater aucune collecte de clé
    void OnEnable()
    {
        // S'abonne à l'événement statique de Target_Key
        // "+=" ajoute CollectKey à la liste des écouteurs
        Target_Key.OnKeyCollected += CollectKey;
    }

    // OnDisable : Appelé quand le script est désactivé
    // Si on ne le fait pas, le KeyManager continuerait d'écouter même
    // après avoir changé de scène, causant des erreurs
    void OnDisable()
    {
        // Se désabonne de l'événement statique
        // "-=" retire CollectKey de la liste des écouteurs
        Target_Key.OnKeyCollected -= CollectKey;
    }
    void Start()
    {
        // Réinitialise le compteur à 0
        keysCollected = 0;

        // Notifie que le compteur est à 0 (pour initialiser l'UI si besoin)
        OnKeysCollected?.Invoke(keysCollected);
    }

    // CollectKey : Appelé automatiquement quand une clé est collectée
    // 1. Incrémente le compteur
    // 2. Affiche un message dans la Console Unity
    // 3. Déclenche l'événement OnKeysCollected (les portes écoutent)
    // 4. Si toutes les clés sont collectées, affiche un message spécial
    private void CollectKey()
    {
        keysCollected++; // Ajoute 1 au compteur

        // Déclenche l'événement avec le nouveau nombre de clés
        // Les KeyDoor écoutent cet événement et vérifient si c'est suffisant
        OnKeysCollected?.Invoke(keysCollected);

        // Si toutes les clés ont été collectées
        if (keysCollected >= totalKeysRequired)
        {
            Debug.Log("Toutes les clés collectées ! La porte s'ouvre...");
        }
    }

    // ResetKeys : Réinitialise le compteur de clés
    // Utilisé si on veut recommencer le niveau ou réinitialiser le système
    public void ResetKeys()
    {
        keysCollected = 0;
        OnKeysCollected?.Invoke(keysCollected);
    }
}
