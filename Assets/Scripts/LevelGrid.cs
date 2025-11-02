using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [Header("Configuration de la grille")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float tileSize = 1f;

    [Header("Préfabs de tuiles")]
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallTilePrefab;
    [SerializeField] private GameObject goalTilePrefab;

    private TileType[,] grid;
    private Vector2Int goalPosition;

    void Awake()
    {
        grid = new TileType[gridWidth, gridHeight];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = TileType.Floor;
            }
        }
    }

    public void SetTile(int x, int y, TileType tileType)
    {
        if (IsWithinBounds(x, y))
        {
            grid[x, y] = tileType;

            if (tileType == TileType.Goal)
            {
                goalPosition = new Vector2Int(x, y);
            }
        }
    }

    public TileType GetTile(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            return grid[x, y];
        }
        return TileType.Empty;
    }

    public bool CanMoveToTile(Vector2Int position)
    {
        if (!IsWithinBounds(position.x, position.y))
            return false;

        TileType tileType = grid[position.x, position.y];
        return tileType == TileType.Floor || tileType == TileType.Goal;
    }

    public bool IsGoalTile(Vector2Int position)
    {
        return position == goalPosition;
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    public void GenerateVisualGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tilePrefab = null;

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

                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
