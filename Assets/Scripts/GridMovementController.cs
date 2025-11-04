using UnityEngine;
using UnityEngine.InputSystem;

// Contrôle le déplacement du joueur case par case (grid-based movement)
// COMMENT ÇA FONCTIONNE ?
// 1. Le joueur appuie sur une touche directionnelle
// 2. On vérifie si la case suivante est praticable (pas un mur)
// 3. Si oui, on déplace progressivement le joueur vers cette case
// 4. On vérifie si le joueur a atteint le Goal ou collecté quelque chose

public class GridMovementController : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tileSize = 1f;

    // Variables de position et d'état
    private Vector2Int currentGridPosition;  // Position actuelle dans la grille (en coordonnées de cases)
    private Vector3 targetPosition;          // Position 3D vers laquelle on se déplace
    private bool isMoving = false;           // true si le joueur est en train de se déplacer
    private bool reachedGoal = false;        // true si ce mouvement atteint le Goal
    private bool isLastMove = false;         // true si c'est le dernier coup disponible (et qu'on n'a pas gagné)

    // Références aux autres composants
    private LevelGrid levelGrid;         // Référence à la grille pour vérifier les cases
    private MoveCounter moveCounter;     // Référence au compteur de coups

    // Référence à l'action d'input du New Input System
    private InputAction moveAction;

    // Awake : Appelé AVANT Start
    // On récupère l'action "Move" du New Input System ici car on doit
    // le faire avant que les inputs soient utilisés
    void Awake()
    {
        // Récupère le composant PlayerInput attaché au joueur
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            // Récupère l'action "Move" depuis l'Input Action Asset
            moveAction = playerInput.actions["Move"];
        }
    }

    void Start()
    {
        // Trouve les composants dans la scène
        levelGrid = FindFirstObjectByType<LevelGrid>();
        moveCounter = FindFirstObjectByType<MoveCounter>();

        // Calcule la position de grille actuelle depuis la position 3D du joueur
        // Mathf.RoundToInt arrondit à l'entier le plus proche
        // On divise par tileSize pour convertir position monde ? position grille
        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.z / tileSize)
        );

        // Au départ, la cible est la position actuelle (pas de mouvement)
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget(); // Continue le mouvement en cours
        }
        else
        {
            HandleInput(); // Écoute les inputs du joueur
        }
    }

    // HandleInput : Gère les entrées du joueur (clavier, manette, etc.)
    private void HandleInput()
    {
        // Si plus de coups disponibles, on n'accepte plus d'inputs
        if (moveCounter != null && moveCounter.MovesRemaining <= 0)
            return;

        Vector2 input = Vector2.zero; // Valeur par défaut

        // Lit la valeur de l'action "Move" (retourne un Vector2)
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }

        Vector2Int direction = Vector2Int.zero; // Direction finale en cases

        // Détermine la direction DOMINANTE
        // Si l'input horizontal (x) est plus fort que le vertical (y)
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            // Mouvement horizontal
            if (input.x > 0.5f) direction = Vector2Int.right;       // Droite : (1, 0)
            else if (input.x < -0.5f) direction = Vector2Int.left;  // Gauche : (-1, 0)
        }
        else
        {
            // Mouvement vertical
            if (input.y > 0.5f) direction = new Vector2Int(0, 1);      // Haut : (0, 1)
            else if (input.y < -0.5f) direction = new Vector2Int(0, -1); // Bas : (0, -1)
        }

        // Si une direction a été détectée, tente le mouvement
        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }

    // TryMove : Essaie de se déplacer dans une direction
    // 1. Calcule la nouvelle position dans la grille
    // 2. Vérifie si cette case est praticable (via LevelGrid)
    // 3. Si oui, initialise le mouvement progressif
    // 4. Consomme un coup du MoveCounter
    // On vérifie AVANT de bouger si c'est possible (pas de mur)
    private void TryMove(Vector2Int direction)
    {
        // Calcule la nouvelle position en ajoutant la direction
        Vector2Int newGridPosition = currentGridPosition + direction;

        // Vérifie si on peut se déplacer sur cette case
        if (levelGrid != null && levelGrid.CanMoveToTile(newGridPosition))
        {
            // Met à jour la position logique dans la grille
            currentGridPosition = newGridPosition;

            // Calcule la position 3D cible dans le monde Unity
            targetPosition = new Vector3(
                currentGridPosition.x * tileSize,
                transform.position.y,           // Garde la même hauteur Y
                currentGridPosition.y * tileSize
            );

            // Active le mouvement progressif
            isMoving = true;

            // Vérifie si cette nouvelle position est le Goal
            bool isGoalReached = levelGrid.IsGoalTile(newGridPosition);
            reachedGoal = isGoalReached;

            // Gère le compteur de coups
            if (moveCounter != null)
            {
                // Vérifie si c'est le dernier coup disponible
                bool willBeLastMove = moveCounter.MovesRemaining == 1;
                isLastMove = willBeLastMove && !isGoalReached;

                // Consomme un coup
                // deferGameOver: true = on reporte le Game Over après la fin du mouvement
                moveCounter.UseMove(isGoalReached, deferGameOver: true);
            }
        }
    }

    // MoveTowardsTarget : Déplace progressivement vers la position cible
    // Utilise Vector3.MoveTowards pour un mouvement fluide et linéaire
    // vers la case cible. Quand on arrive, on vérifie :
    // - Si on a collecté quelque chose
    // - Si on a atteint le Goal (victoire)
    // - Si c'était le dernier coup (défaite)
    // On attend la FIN du mouvement avant de déclencher victoire/défaite
    // pour que le joueur voie bien son personnage arriver sur la case
    private void MoveTowardsTarget()
    {
        // Vector3.MoveTowards déplace progressivement vers la cible
        // moveSpeed * Time.deltaTime = distance parcourue cette frame
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Vérifie si on est arrivé à destination (distance < 0.01)
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Snap exactement à la position (pour éviter les imprécisions)
            transform.position = targetPosition;
            isMoving = false; // Mouvement terminé

            // Vérifie s'il y a des collectibles sur cette case
            CheckForCollectibles();

            // Si on a atteint le Goal : Victoire !
            if (reachedGoal)
            {
                reachedGoal = false; // Reset le flag
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.LevelComplete(); // Déclenche la victoire
                }
            }
            // Sinon, si c'était le dernier coup : Game Over
            else if (isLastMove)
            {
                isLastMove = false; // Reset le flag
                if (moveCounter != null && moveCounter.MovesRemaining <= 0)
                {
                    GameManager gameManager = FindFirstObjectByType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.GameOver(); // Déclenche la défaite
                    }
                }
            }
        }
    }
 
    // Utilisé au début du niveau pour placer le joueur
    // Réinitialise tous les états de mouvement
    public void ResetPosition(Vector2Int gridPosition)
    {
        currentGridPosition = gridPosition;
        targetPosition = new Vector3(
            gridPosition.x * tileSize,
            transform.position.y,
            gridPosition.y * tileSize
        );
        transform.position = targetPosition;
        isMoving = false;
        reachedGoal = false;
        isLastMove = false;
    }

    // CheckForCollectibles : Détecte les collectibles sur la case actuelle
    // Utilise Physics.OverlapSphere pour détecter tous les colliders
    // dans un rayon de 0.3 unités autour du joueur
    // On utilise la physique Unity pour détecter les objets proches
    // plutôt que de vérifier case par case dans la grille
    // C'est plus flexible et permet d'avoir plusieurs objets sur une case
    private void CheckForCollectibles()
    {
        // Trouve tous les colliders dans un rayon de 0.3 unités
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.3f);

        // Parcourt chaque collider détecté
        foreach (Collider col in colliders)
        {
            // Ignore le collider du joueur lui-même
            if (col.gameObject != gameObject)
            {
                // Vérifie si c'est un Target_Soft
                Target_Soft softTarget = col.GetComponent<Target_Soft>();
                if (softTarget != null)
                {
                    softTarget.OnPlayerCollect(gameObject); // Collecte
                    continue; // Passe au suivant
                }

                // Vérifie si c'est un Target_Hard
                Target_Hard hardTarget = col.GetComponent<Target_Hard>();
                if (hardTarget != null)
                {
                    hardTarget.OnPlayerCollect(gameObject); // Collecte
                    continue;
                }

                // Vérifie si c'est une Target_Key
                Target_Key keyTarget = col.GetComponent<Target_Key>();
                if (keyTarget != null)
                {
                    keyTarget.OnPlayerCollect(gameObject); // Collecte
                }
            }
        }
    }
}
