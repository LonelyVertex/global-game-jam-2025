using UnityEngine;

public class SimpleInputController : MonoBehaviour
{
    public PlayerController playerController;

    private Vector4 inputVector = Vector4.zero;

    void Update()
    {
        inputVector = Vector4.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.x = 1;   
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = 1;
        }


        if (Input.GetKey(KeyCode.A))
        {
            inputVector.z = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputVector.w = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerController.Fire();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerController.SetUnderwater(!playerController.IsUnderwater());
        }

        playerController.SetInputVector(inputVector);
    }
}
