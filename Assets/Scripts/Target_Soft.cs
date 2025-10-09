using System.Collections;
using UnityEngine;

public class Target_Soft : MonoBehaviour
{
    [SerializeField] private int _targetValue = 1;
    [SerializeField] private float _shadowDuration = 3f;
    [SerializeField] private GameObject _particuleEffect;
    private float _shadowTimer = 0f;
    private bool _isInShadow = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player_Collect>() != null)
        {
            other.gameObject.GetComponent<Player_Collect>().UpdateScore(_targetValue);
            //Destroy(gameObject);
            //TODO : Hide Target
            ToggleVisibilty(false);
            //TODO : Start Timer
            //_isInShadow = true;
            //Instantiate(_particuleEffect.transform.position,Quartenion.identity);
            StartCoroutine(routine:ShadowTimerControl());
        }
    }

    private void ToggleVisibilty(bool newVisibility)
    {
        GetComponent<MeshRenderer>().enabled = newVisibility;
        GetComponent<Collider>().enabled = newVisibility;
    }
    //TODO : Timer by deltatime

    /*private void Update()
    {
        if (_isInShadow)
        {
            _shadowTimer += Time.deltaTime;

            if (_shadowTimer >= _shadowDuration)
            {
                //TODO : Show Target
                ToggleVisibilty(true);
                //TODO : Stop Timer
                _shadowTimer = 0f;
                _isInShadow = false;
            }
        }
    }*/


    //TODO : Timer by coroutine
    private IEnumerator ShadowTimerControl()
    {
        //yield return new WaitEndOfFrame();
        yield return new WaitForSeconds(_shadowDuration);
        ToggleVisibilty(true);
    }

}