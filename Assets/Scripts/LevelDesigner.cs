using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private LevelGrid levelGrid;

    [Header("Design du niveau")]
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
    [SerializeField] private Vector2Int playerStartPosition = new Vector2Int(1, 1);

    void Start()
    {
        if (levelGrid == null)
        {
            levelGrid = GetComponent<LevelGrid>();
        }

        CreateLevelFromLayout();
        PositionPlayer();
        levelGrid.GenerateVisualGrid();
    }

    private void CreateLevelFromLayout()
    {
        string[] lines = levelLayout.Split('\n');

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y].Trim();
            for (int x = 0; x < line.Length; x++)
            {
                char tile = line[x];
                int gridY = lines.Length - 1 - y;

                switch (tile)
                {
                    case '#':
                        levelGrid.SetTile(x, gridY, TileType.Wall);
                        break;
                    case '.':
                        levelGrid.SetTile(x, gridY, TileType.Floor);
                        break;
                    case 'G':
                        levelGrid.SetTile(x, gridY, TileType.Goal);
                        break;
                }
            }
        }
    }

    private void PositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("=== Player ===");
        }

        if (player != null)
        {
            GridMovementController gridMovement = player.GetComponent<GridMovementController>();
            if (gridMovement != null)
            {
                Vector3 startPos = new Vector3(
                    playerStartPosition.x,
                    player.transform.position.y,
                    playerStartPosition.y
                );
                player.transform.position = startPos;
                gridMovement.ResetPosition(playerStartPosition);
            }
        }
    }
}
