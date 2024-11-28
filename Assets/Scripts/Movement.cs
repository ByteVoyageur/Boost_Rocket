using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] InputAction thrust;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        thrust.Enable();
    }

    private void FixedUpdate()
    {
        if(thrust.IsPressed())
        {
            rb.AddRelativeForce(0, 10, 0);
            Debug.Log("Dang, I need some space");
        }
    }
}
