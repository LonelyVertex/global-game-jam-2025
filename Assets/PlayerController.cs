using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float acceleration = 0.3f;
    public float rotationSpeed = 0.1f;

    public Transform bodyTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Accelerate();
        }

        if (Input.GetKey(KeyCode.S))
        {
            Decelerate();
        }


        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
    }

    public void Accelerate()
    {
        Debug.Log("Accelerated"); 
        Debug.Log(rb.transform.up);
        rb.AddForce(rb.transform.up*acceleration, ForceMode2D.Force);
    }
    
    public void Decelerate()
    {
        Debug.Log("Decelerated"); 
        rb.AddForce(-rb.transform.up* acceleration, ForceMode2D.Force);
    }
    public void RotateLeft()
    {
        Debug.Log("Rotated Left");
        rb.AddTorque(rotationSpeed, ForceMode2D.Force);
    }

        public void RotateRight()
    {
        Debug.Log("Rotated Right");
        rb.AddTorque(-rotationSpeed, ForceMode2D.Force);
    }
}
