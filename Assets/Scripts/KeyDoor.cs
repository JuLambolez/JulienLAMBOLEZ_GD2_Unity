using UnityEngine;

// Porte qui bloque le passage du joueur jusqu'à ce qu'il collecte
// toutes les clés nécessaires
// - Bloque la case dans la grille (le joueur ne peut pas passer)
// - Écoute le KeyManager pour savoir combien de clés sont collectées
// - S'ouvre (débloque la case) quand toutes les clés sont collectées
// - Animation de disparition progressive
// 1. Au Start : bloque la case dans LevelGrid
// 2. À chaque clé collectée : vérifie si c'est suffisant
// 3. Quand assez de clés : débloque la case et lance l'animation
// 4. L'animation fait disparaître la porte progressivement

public class KeyDoor : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int keysRequired = 3;   // Nombre de clés nécessaires
    [SerializeField] private float tileSize = 1f;    // Taille d'une case (pour calculs de position)

    [Header("Effets visuels")]
    [SerializeField] private bool animateDisappear = true;     // Animer la disparition ?
    [SerializeField] private float disappearDuration = 1f;     // Durée de l'animation (secondes)

    // Références aux composants
    private KeyManager keyManager;
    private LevelGrid levelGrid;

    // Variables d'état
    private bool isDisappearing = false;  // Animation en cours ?
    private bool isDoorOpen = false;      // Porte ouverte ?
    private float disappearTimer = 0f;    // Timer pour l'animation

    // Références aux composants Unity
    private Vector3 originalScale;        // Échelle originale (pour l'animation)
    private MeshRenderer meshRenderer;    // Pour changer la transparence
    private BoxCollider boxCollider;      // Pour désactiver les collisions

    void Start()
    {
        // Trouve les composants dans la scène
        keyManager = FindFirstObjectByType<KeyManager>();
        levelGrid = FindFirstObjectByType<LevelGrid>();

        // Mémorise l'échelle originale
        originalScale = transform.localScale;

        // Récupère les composants sur ce GameObject
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();

        // Configure le collider pour bloquer physiquement
        // isTrigger = false : le joueur ne peut pas traverser
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }

        // S'abonne à l'événement du KeyManager
        if (keyManager != null)
        {
            keyManager.OnKeysCollected += CheckKeys;

            // Vérifie immédiatement l'état actuel
            // (au cas où des clés auraient déjà été collectées)
            CheckKeys(keyManager.KeysCollected);
        }

        // Bloque la case dans la grille
        BlockTile();
    }

    // OnDestroy : Nettoyage avant destruction
    // Se désabonne de l'événement pour éviter les erreurs
    void OnDestroy()
    {
        if (keyManager != null)
        {
            keyManager.OnKeysCollected -= CheckKeys;
        }
    }

    // Update : Animation de disparition
    // 1. Incrémente le timer
    // 2. Calcule le pourcentage d'avancement (0 à 1)
    // 3. Réduit progressivement l'échelle (la porte rétrécit)
    // 4. Réduit progressivement l'opacité (la porte devient transparente)
    // 5. Détruit la porte quand l'animation est terminée
    // Lerp (Linear Interpolation) crée une transition fluide
    void Update()
    {
        if (isDisappearing)
        {
            disappearTimer += Time.deltaTime; // Ajoute le temps écoulé

            // Calcule le pourcentage d'avancement (0 = début, 1 = fin)
            float progress = disappearTimer / disappearDuration;

            if (animateDisappear)
            {
                // ANIMATION D'ÉCHELLE : rétrécit progressivement vers 0
                // Lerp = interpolation linéaire entre deux valeurs
                // Lerp(A, B, 0.5) = valeur à mi-chemin entre A et B
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

                // ANIMATION DE TRANSPARENCE : devient progressivement invisible
                if (meshRenderer != null)
                {
                    Color color = meshRenderer.material.color;
                    color.a = 1f - progress; // Alpha de 1 (opaque) à 0 (transparent)
                    meshRenderer.material.color = color;
                }
            }

            // Quand l'animation est terminée (progress >= 1)
            if (progress >= 1f)
            {
                Destroy(gameObject); // Détruit la porte
            }
        }
    }

    // CheckKeys : Vérifie si assez de clés ont été collectées
    // Appelé automatiquement par l'événement OnKeysCollected du KeyManager
    // à chaque fois qu'une clé est collectée
    private void CheckKeys(int currentKeys)
    {
        // Si assez de clés ET que la porte n'est pas déjà ouverte
        if (currentKeys >= keysRequired && !isDoorOpen)
        {
            OpenDoor(); // Ouvre la porte
        }
    }

    // OpenDoor : Ouvre la porte
    // 1. Marque la porte comme ouverte
    // 2. Débloque la case dans la grille (le joueur peut passer)
    // 3. Désactive le collider (plus de collision physique)
    // 4. Lance l'animation de disparition
    // On DOIT débloquer la case dans LevelGrid, sinon le joueur
    // ne pourra toujours pas passer même si la porte est invisible
    private void OpenDoor()
    {
        isDoorOpen = true; // Marque comme ouverte
        Debug.Log($"Porte ouverte ! {keysRequired}/{keysRequired} clés collectées.");

        // Débloque la case dans la grille (CRITIQUE !)
        UnblockTile();

        // Désactive le collider pour qu'il n'y ait plus de collision
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Lance l'animation de disparition
        if (animateDisappear)
        {
            isDisappearing = true;
            disappearTimer = 0f;

            // Configure le matériau pour supporter la transparence
            // Ces lignes changent le mode de rendu du matériau
            if (meshRenderer != null && meshRenderer.material != null)
            {
                Material mat = meshRenderer.material;

                // Configure le shader pour le mode transparent
                // Ces valeurs sont spécifiques à Unity 6 URP
                mat.SetFloat("_Surface", 1); // 1 = Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.renderQueue = 3000; // Queue de rendu pour les objets transparents
            }
        }
        else
        {
            // Si pas d'animation, détruit directement
            Destroy(gameObject);
        }
    }

    // BlockTile : Bloque la case dans la grille
    // Appelé au Start pour empêcher le joueur de passer
    // 1. Convertit la position 3D de la porte en coordonnées de grille
    // 2. Appelle LevelGrid.SetTileWalkable(position, false)
    // 3. La case devient un Wall dans la grille
    private void BlockTile()
    {
        if (levelGrid != null)
        {
            // Convertit position monde ? position grille
            Vector2Int tilePos = WorldToGridPosition(transform.position);

            // Marque la case comme non-praticable (Wall)
            levelGrid.SetTileWalkable(tilePos, false);

            Debug.Log($"Tile bloquée à {tilePos}");
        }
    }

    // UnblockTile : Débloque la case dans la grille
    // Appelé quand la porte s'ouvre
    // On modifie dynamiquement la grille pendant le jeu
    // C'est essentiel pour que le système grid-based détecte
    // que la case est maintenant praticable
    private void UnblockTile()
    {
        if (levelGrid != null)
        {
            // Convertit position monde ? position grille
            Vector2Int tilePos = WorldToGridPosition(transform.position);

            // Marque la case comme praticable (Floor)
            levelGrid.SetTileWalkable(tilePos, true);

            Debug.Log($"Tile débloquée à {tilePos}");
        }
    }

    // WorldToGridPosition : Convertit position 3D ? coordonnées de grille
    // RETOURNE : Vector2Int - coordonnées de grille (x, y)
    // Divise par tileSize et arrondit à l'entier le plus proche
    // Exemple : worldPos (5.3, 0, 7.8) avec tileSize=1 ? (5, 8)
    // La grille utilise des coordonnées entières (0, 1, 2, 3...)
    // La position 3D utilise des coordonnées flottantes (0.5, 1.3, 2.7...)
    // On doit convertir entre les deux systèmes
    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / tileSize), // Coordonnée X de grille
            Mathf.RoundToInt(worldPos.z / tileSize)  // Coordonnée Y de grille (Z en 3D)
        );
    }
}
