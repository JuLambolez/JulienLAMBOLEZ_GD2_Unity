using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _movement;
    private Vector3 _grappinDirection;
    private Vector3 _grappinHit;
    [SerializeField] private float _speed = 6.0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");
        _movement = new Vector3(_horizontalMovement, 0.0f, _verticalMovement);
        GrappinUpdateDirection(_movement); //Mise à jour de la direction du tir du grapin
        _movement.Normalize();
        _movement *= _speed;
        _movement.y = _rb.linearVelocity.y;
        if (_rb != null)
        {
            _rb.linearVelocity = _movement;
        }
        else
        {
            Debug.LogError("No RigidBody Attached !");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            TryThrowGrappin();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            ThrowGrappin();
        }

    }

    private void GrappinUpdateDirection(Vector3 direction)
    {

        if (direction.sqrMagnitude > 0.1f)
        {
            _grappinDirection = direction;
        }
    }

    private void TryThrowGrappin()
    {
        RaycastHit Hit;
        if(Physics.Raycast(transform.position,_grappinDirection, out Hit, maxDistance:100f))
        {
            _grappinHit = Hit.point+Hit.normal*1.5f;
        }
    }

    private void ThrowGrappin()
    {
        transform.position = _grappinHit;
        _grappinDirection = Vector3.zero;
    }
}
