using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    private int _spawnPointIndex = 0;
    private void OnEnable()
    {
        Player_Collect.OnTargetCollected += SpawnNewWall;
    }

    private void OnDisable()
    {
        Player_Collect.OnTargetCollected -= SpawnNewWall;
    }

    private void SpawnNewWall(int score)
    {
        if (_spawnPointIndex >= _spawnPoints.Length)
        {
            return;
        }
        Instantiate(_wallPrefab, _spawnPoints[_spawnPointIndex].position, _spawnPoints[_spawnPointIndex].rotation);
        _spawnPointIndex++;

    }
}
