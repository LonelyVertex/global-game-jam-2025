using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float acceleration = 0.3f;
    public float rotationSpeed = 0.1f;

    public Transform bodyTransform;

/// <summary>
/// (acceleration, deceleartion, rotationLeft, rotationRight)
/// </summary>
    private Vector4 inputVector = Vector4.zero;

    void FixedUpdate()
    {
        ApplyInputVector();
    }

    public void SetInputVector(Vector4 input)
    {
        inputVector = input;
    }

    private void ApplyInputVector()
    {
        if (inputVector.x > 0)
        {
            Accelerate();
        }

        if (inputVector.y > 0)
        {
            Decelerate();
        }

        if (inputVector.z > 0)
        {
            RotateLeft();
        }

        if (inputVector.w > 0)
        {
            RotateRight();
        }
    }

    private void Accelerate()
    {
        Debug.Log("Accelerated"); 
        rb.AddForce(rb.transform.up*acceleration, ForceMode2D.Force);
    }
    
    private void Decelerate()
    {
        Debug.Log("Decelerated"); 
        rb.AddForce(-rb.transform.up* acceleration, ForceMode2D.Force);
    }
    private void RotateLeft()
    {
        Debug.Log("Rotated Left");
        rb.AddTorque(rotationSpeed, ForceMode2D.Force);
    }

    private void RotateRight()
    {
        Debug.Log("Rotated Right");
        rb.AddTorque(-rotationSpeed, ForceMode2D.Force);
    }
}
