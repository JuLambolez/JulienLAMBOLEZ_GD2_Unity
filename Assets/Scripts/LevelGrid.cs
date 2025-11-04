using UnityEngine;

// Gère la grille de jeu (toutes les cases du niveau)

public class LevelGrid : MonoBehaviour
{
    [Header("Configuration de la grille")]
    [SerializeField] private int gridWidth = 10;   // Largeur de la grille (nombre de cases en X)
    [SerializeField] private int gridHeight = 10;  // Hauteur de la grille (nombre de cases en Z)
    [SerializeField] private float tileSize = 1f;  // Taille d'une case en unités Unity

    [Header("Préfabs de tuiles")]
    // Les préfabs à instancier pour afficher visuellement la grille
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallTilePrefab;
    [SerializeField] private GameObject goalTilePrefab;

    // Tableau 2D qui stocke le type de chaque case
    // grid[x, y] donne le type de la case à la position (x, y)
    private TileType[,] grid;

    // Position de la case Goal dans la grille
    private Vector2Int goalPosition;


    //Awake au lieu de Start pour garantir que la grille est créée 
    //AVANT que d'autres scripts essaient de l'utiliser
    void Awake()
    {
        // Crée un tableau 2D de taille gridWidth x gridHeight
        grid = new TileType[gridWidth, gridHeight];
        InitializeGrid(); // Remplit toutes les cases avec du Floor par défaut
    }

    // Toutes les cases sont initialement du sol, puis le LevelDesigner
    // modifiera certaines cases pour créer le design du niveau
    private void InitializeGrid()
    {
        // Double boucle pour parcourir toutes les cases
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = TileType.Floor; // Toutes les cases sont du sol au départ
            }
        }
    }

    // SetTile : Modifie le type d'une case de la grille :
    // - x, y : coordonnées de la case
    // - tileType : le nouveau type à assigner (Wall, Floor, Goal, etc.)
    // Utilisé par LevelDesigner pour construire le niveau

    public void SetTile(int x, int y, TileType tileType)
    {
        // Vérifie que la position est bien dans les limites de la grille
        if (IsWithinBounds(x, y))
        {
            grid[x, y] = tileType; // Change le type de la case

            // Si c'est le Goal, on mémorise sa position
            // (pour vérifier plus tard si le joueur a atteint le Goal)
            if (tileType == TileType.Goal)
            {
                goalPosition = new Vector2Int(x, y);
            }
        }
    }

    // GetTile : Récupère le type d'une case
    // RETOURNE : Le type de la case à la position (x, y)
    public TileType GetTile(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            return grid[x, y]; // Retourne le type de case
        }
        return TileType.Empty; // Hors limites = Empty
    }

    // CanMoveToTile : Vérifie si le joueur peut se déplacer sur une case
    // RETOURNE : true si le joueur peut marcher sur cette case, false sinon
    public bool CanMoveToTile(Vector2Int position)
    {
        // Vérifie d'abord que la position est dans la grille
        if (!IsWithinBounds(position.x, position.y))
            return false;

        // Récupère le type de cette case
        TileType tileType = grid[position.x, position.y];

        // Le joueur peut marcher sur Floor ou Goal
        return tileType == TileType.Floor || tileType == TileType.Goal;
    }

    // IsGoalTile : Vérifie si une position est la case Goal
    // Utilisé pour savoir si le joueur a atteint le Goal et gagné
    public bool IsGoalTile(Vector2Int position)
    {
        return position == goalPosition; // Compare avec la position mémorisée du Goal
    }

    // SetTileWalkable : Change dynamiquement si une case est praticable
    // walkable : true = rendre la case praticable, false = bloquer la case
    // Utilisé par KeyDoor pour bloquer une case (quand la porte est fermée)
    // puis la débloquer (quand le joueur a collecté toutes les clés)
    public void SetTileWalkable(Vector2Int position, bool walkable)
    {
        if (IsWithinBounds(position.x, position.y))
        {
            if (walkable)
            {
                // Rendre la case praticable : transformer Wall ou Empty en Floor
                if (grid[position.x, position.y] == TileType.Wall || grid[position.x, position.y] == TileType.Empty)
                {
                    grid[position.x, position.y] = TileType.Floor;
                }
            }
            else
            {
                // Bloquer la case : transformer Floor en Wall
                if (grid[position.x, position.y] == TileType.Floor)
                {
                    grid[position.x, position.y] = TileType.Wall;
                }
            }
        }
    }

    // IsWithinBounds : Vérifie si une position est dans la grille
    // RETOURNE : true si (x, y) est dans les limites de la grille
    // Permet d'éviter les erreurs "index out of range" qui crasheraient le jeu
    // On vérifie toujours avant d'accéder à grid[x, y]
    private bool IsWithinBounds(int x, int y)
    {
        // x et y doivent être >= 0 ET < à la taille de la grille
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    // GenerateVisualGrid : Crée visuellement la grille dans la scène
    // Parcourt toute la grille et instancie les préfabs correspondants
    // à chaque case (Floor, Wall, Goal)
    // POURQUOI FAIRE COMME ÇA ?
    public void GenerateVisualGrid()
    {
        // Parcourt toutes les cases de la grille
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calcule la position 3D dans le monde Unity
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tilePrefab = null;

                // Sélectionne le bon préfab selon le type de case
                switch (grid[x, y])
                {
                    case TileType.Floor:
                        tilePrefab = floorTilePrefab;
                        break;
                    case TileType.Wall:
                        tilePrefab = wallTilePrefab;
                        break;
                    case TileType.Goal:
                        tilePrefab = goalTilePrefab;
                        break;
                }

                // Si un préfab est défini, on l'instancie
                if (tilePrefab != null)
                {
                    // Instantiate crée une copie du préfab dans la scène
                    // transform = parent de cet objet (pour organiser la hierarchy)
                    Instantiate(tilePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
