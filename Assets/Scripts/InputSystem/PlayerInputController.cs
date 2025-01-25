using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerController playerController;
    InputAction forward;
    InputAction backward;
    InputAction left;
    InputAction right;
    InputAction shoot;
    InputAction dive;
    InputAction jump;
    Vector4 movementVector = new Vector4();
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerGO;

    public void Start()
    {
        var input = new UnityInputSystemActions();
        // Not ideal, I would do it over an Unity event as recommended in https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/ActionAssets.html
        forward = playerInput.actions.FindAction(input.Player.Forward.id);
        backward = playerInput.actions. FindAction(input.Player.Backward.id);
        left = playerInput.actions.FindAction(input.Player.Left.id);
        right = playerInput.actions.FindAction(input.Player.Right.id);
        shoot = playerInput.actions.FindAction(input.Player.Shoot.id);
        dive = playerInput.actions.FindAction(input.Player.Dive.id);
        jump = playerInput.actions.FindAction(input.Player.Jump.id);
        
        playerGO = Instantiate(playerPrefab);
        playerController = playerGO.GetComponent<PlayerController>();
    }

    public void Update()
    {
        var fwd = forward.ReadValue<float>();
        var bck = backward.ReadValue<float>();
        var lft = left.ReadValue<float>();
        var rgt = right.ReadValue<float>();
        var sht = shoot.ReadValue<float>();
        var dv = dive.ReadValue<float>();
        var j = jump.ReadValue<float>();
        
        playerController.SetInputVector(new Vector4(fwd, bck, lft, rgt));
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        Debug.Log($"Forward pressed ({gameObject.name}): {context.ReadValueAsButton()}");
    }

    public void OnBackward(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnDive(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}