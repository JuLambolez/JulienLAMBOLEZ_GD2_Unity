using UnityEngine;

public class Target_Hard : MonoBehaviour
{
    [SerializeField] private int _targetValue = -3;

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
