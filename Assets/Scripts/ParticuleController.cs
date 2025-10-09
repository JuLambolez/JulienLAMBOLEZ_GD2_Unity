using UnityEngine;

public class ParticuleController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,t:5f);
    }
     
}
