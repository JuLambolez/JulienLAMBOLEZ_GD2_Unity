using UnityEngine;

public class Target_Soft : MonoBehaviour
{
    [SerializeField] private int _targetValue = 5;
    [SerializeField] private GameObject _particuleEffect;

    public void OnPlayerCollect(GameObject player)
    {
        Player_Collect collector = player.GetComponent<Player_Collect>();
        if (collector != null)
        {
            collector.UpdateScore(_targetValue);
            Destroy(gameObject);
        }
    }
}
