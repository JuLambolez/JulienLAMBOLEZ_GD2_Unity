using UnityEngine;
using System;

// Collectible spécial : une clé nécessaire pour ouvrir une porte
// Contrairement à Target_Soft/Hard qui modifient le score,
// cette clé déclenche un événement pour le système de portes
// - Animation visuelle (rotation + bobbing)
// - Événement statique pour notifier KeyManager

public class Target_Key : MonoBehaviour
{
    // Événement statique déclenché quand une clé est collectée
    // Le KeyManager écoute cet événement
    public static Action OnKeyCollected;

    [Header("Effets visuels")]
    [SerializeField] private float rotationSpeed = 50f;  // Vitesse de rotation (degrés/seconde)
    [SerializeField] private float bobSpeed = 2f;        // Vitesse du mouvement vertical
    [SerializeField] private float bobHeight = 0.2f;     // Amplitude du mouvement vertical

    private Vector3 startPosition; // Position de départ (pour le bobbing)

    //Mémorise la position initiale
    void Start()
    {
        startPosition = transform.position;
    }

    // Update : Animation visuelle de la clé
    // 1. ROTATION : tourne autour de l'axe Y (vertical)
    // 2. BOBBING : mouvement de haut en bas avec une fonction sinus
    void Update()
    {
        // ROTATION : tourne autour de l'axe Y
        // Vector3.up = (0, 1, 0) = axe Y
        // rotationSpeed * Time.deltaTime = angle de rotation cette frame
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // BOBBING : mouvement vertical sinusoïdal
        // Mathf.Sin(Time.time * bobSpeed) oscille entre -1 et +1
        // Multiplié par bobHeight pour contrôler l'amplitude
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Applique la nouvelle position Y, garde X et Z identiques
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // OnPlayerCollect : Appelé quand le joueur collecte la clé
    // 1. Vérifie que le joueur a bien le composant Player_Collect
    // 2. Déclenche l'événement OnKeyCollected (écouté par KeyManager)
    // 3. Détruit la clé de la scène
    public void OnPlayerCollect(GameObject player)
    {
        // Vérifie que le joueur a le composant Player_Collect
        Player_Collect collector = player.GetComponent<Player_Collect>();

        if (collector != null)
        {
            // Déclenche l'événement : "Une clé a été collectée !"
            OnKeyCollected?.Invoke();

            // Détruit la clé de la scène
            Destroy(gameObject);
        }
    }
}
