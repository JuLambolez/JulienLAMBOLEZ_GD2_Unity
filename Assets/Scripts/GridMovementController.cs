using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovementController : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tileSize = 1f;

    private Vector2Int currentGridPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool reachedGoal = false;
    private bool isLastMove = false;

    private LevelGrid levelGrid;
    private MoveCounter moveCounter;

    private InputAction moveAction;

    void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    void Start()
    {
        levelGrid = FindFirstObjectByType<LevelGrid>();
        moveCounter = FindFirstObjectByType<MoveCounter>();

        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.z / tileSize)
        );

        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
        else
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (moveCounter != null && moveCounter.MovesRemaining <= 0)
            return;

        Vector2 input = Vector2.zero;

        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }

        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > 0.5f) direction = Vector2Int.right;
            else if (input.x < -0.5f) direction = Vector2Int.left;
        }
        else
        {
            if (input.y > 0.5f) direction = new Vector2Int(0, 1);
            else if (input.y < -0.5f) direction = new Vector2Int(0, -1);
        }

        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newGridPosition = currentGridPosition + direction;

        if (levelGrid != null && levelGrid.CanMoveToTile(newGridPosition))
        {
            currentGridPosition = newGridPosition;
            targetPosition = new Vector3(
                currentGridPosition.x * tileSize,
                transform.position.y,
                currentGridPosition.y * tileSize
            );

            isMoving = true;

            bool isGoalReached = levelGrid.IsGoalTile(newGridPosition);
            reachedGoal = isGoalReached;

            if (moveCounter != null)
            {
                bool willBeLastMove = moveCounter.MovesRemaining == 1;
                isLastMove = willBeLastMove && !isGoalReached;

                moveCounter.UseMove(isGoalReached, deferGameOver: true);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;

            CheckForCollectibles();

            if (reachedGoal)
            {
                reachedGoal = false;
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.LevelComplete();
                }
            }
            else if (isLastMove)
            {
                isLastMove = false;
                if (moveCounter != null && moveCounter.MovesRemaining <= 0)
                {
                    GameManager gameManager = FindFirstObjectByType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.GameOver();
                    }
                }
            }
        }
    }

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

    private void CheckForCollectibles()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.3f);

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                Target_Soft softTarget = col.GetComponent<Target_Soft>();
                if (softTarget != null)
                {
                    softTarget.OnPlayerCollect(gameObject);
                    continue;
                }

                Target_Hard hardTarget = col.GetComponent<Target_Hard>();
                if (hardTarget != null)
                {
                    hardTarget.OnPlayerCollect(gameObject);
                }
            }
        }
    }
}
