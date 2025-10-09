using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    void Update()
    {
        transform.position = _player.transform.position;
        
    }
}
