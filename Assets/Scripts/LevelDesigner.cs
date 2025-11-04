using UnityEngine;

// Permet de créer des niveaux en utilisant du texte ASCII
// Plutôt que de placer manuellement chaque case, on dessine le niveau
// avec des caractères : # = mur, . = sol, G = goal
// - Facile de visualiser le niveau dans l'Inspector
// - Rapide de créer/modifier des niveaux
// - Pas besoin de placer des centaines d'objets à la main
// 1. On écrit le layout en texte dans l'Inspector
// 2. Au Start, le script lit ce texte ligne par ligne
// 3. Pour chaque caractère, il crée la case correspondante
// 4. Il positionne le joueur
// 5. Il génère les visuels de la grille

public class LevelDesigner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private LevelGrid levelGrid; // Référence à la grille

    [Header("Design du niveau")]
    // TextArea crée une grande zone de texte dans l'Inspector
    // (10, 20) = minimum 10 lignes, maximum 20 lignes
    [TextArea(10, 20)]
    [SerializeField]
    private string levelLayout =
        "##########\n" +
        "#........#\n" +
        "#.##..##.#\n" +
        "#........#\n" +
        "#..####..#\n" +
        "#........#\n" +
        "#.##..##.#\n" +
        "#........#\n" +
        "#.......G#\n" +
        "##########";

    [Header("Position du joueur")]
    // Position de départ du joueur dans la grille
    [SerializeField] private Vector2Int playerStartPosition = new Vector2Int(1, 1);

    //Construction du niveau
    void Start()
    {
        // Si pas de référence LevelGrid dans l'Inspector, cherche sur ce GameObject
        if (levelGrid == null)
        {
            levelGrid = GetComponent<LevelGrid>();
        }

        // Étapes de construction du niveau
        CreateLevelFromLayout();      // 1. Crée la grille depuis le texte
        PositionPlayer();             // 2. Place le joueur
        levelGrid.GenerateVisualGrid(); // 3. Génère les visuels
    }

    // CreateLevelFromLayout : Convertit le texte en grille
    // 1. Split('\n') découpe le texte en lignes
    // 2. Pour chaque ligne, on lit chaque caractère
    // 3. Selon le caractère, on crée le type de case correspondant
    // Dans le texte, la première ligne est en HAUT
    // Dans Unity, Y=0 est en BAS
    // On inverse donc : gridY = lines.Length - 1 - y
    private void CreateLevelFromLayout()
    {
        // Split('\n') découpe le texte en un tableau de lignes
        // Exemple : "AB\nCD" ? ["AB", "CD"]
        string[] lines = levelLayout.Split('\n');

        // Parcourt chaque ligne (axe Y)
        for (int y = 0; y < lines.Length; y++)
        {
            // Trim() enlève les espaces au début/fin
            string line = lines[y].Trim();

            // Parcourt chaque caractère de la ligne (axe X)
            for (int x = 0; x < line.Length; x++)
            {
                char tile = line[x]; // Le caractère à cette position

                // INVERSION DE Y
                // Texte :        Unity :
                // y=0 (haut)     gridY=9 (haut)
                // y=1            gridY=8
                // y=9 (bas)      gridY=0 (bas)
                int gridY = lines.Length - 1 - y;

                // Selon le caractère, définit le type de case
                switch (tile)
                {
                    case '#':
                        // '#' = Mur
                        levelGrid.SetTile(x, gridY, TileType.Wall);
                        break;
                    case '.':
                        // '.' = Sol
                        levelGrid.SetTile(x, gridY, TileType.Floor);
                        break;
                    case 'G':
                        // 'G' = Goal (objectif)
                        levelGrid.SetTile(x, gridY, TileType.Goal);
                        break;
                        // Si le caractère n'est pas reconnu, on l'ignore
                        // On pourrait ajouter d'autres cas : 'K' = clé, 'D' = porte, etc.
                }
            }
        }
    }

    // PositionPlayer : Place le joueur à la position de départ
    // 1. Cherche le GameObject du joueur par tag ou nom
    // 2. Calcule sa position 3D depuis les coordonnées de grille
    // 3. Appelle ResetPosition sur GridMovementController
    // On cherche d'abord par tag (plus propre)
    // Si pas trouvé, on cherche par nom (fallback)
    // ResetPosition synchronise la position logique et physique
    private void PositionPlayer()
    {
        // Cherche le joueur par tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Si pas trouvé par tag, cherche par nom
        if (player == null)
        {
            player = GameObject.Find("=== Player ===");
        }

        if (player != null)
        {
            // Récupère le composant de mouvement
            GridMovementController gridMovement = player.GetComponent<GridMovementController>();

            if (gridMovement != null)
            {
                // Calcule la position 3D depuis les coordonnées de grille
                // playerStartPosition = (1, 1) en grille
                // startPos = (1, Y_actuel, 1) en 3D
                Vector3 startPos = new Vector3(
                    playerStartPosition.x,           // X de grille
                    player.transform.position.y,     // Garde la hauteur Y actuelle
                    playerStartPosition.y            // Y de grille devient Z en 3D
                );

                // Place physiquement le joueur
                player.transform.position = startPos;

                // Synchronise la position logique dans GridMovementController
                gridMovement.ResetPosition(playerStartPosition);
            }
        }
    }
}
