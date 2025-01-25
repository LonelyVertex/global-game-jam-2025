using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerInputSystemManager : MonoBehaviour
{
    private static class ControlScheme
    {
        public const string KEYBOARD_1 = "Keyboard1";
        public const string KEYBOARD_2 = "Keyboard2";
        public const string KEYBOARD_3 = "Keyboard3";
        public const string KEYBOARD_4 = "Keyboard4";
        public const string GAMEPAD = "Gamepad";
    }

    [SerializeField] PlayerInputManager playerInputManager;
    HashSet<KeyValuePair<InputDevice, string>> pairedDevices = new ();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameStateManager gameStateManager;
    

    void Start()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
        playerInputManager.playerPrefab = playerPrefab;
        Debug.Log($"Joining enabled: {playerInputManager.joiningEnabled}");
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        Debug.Log($"Left player: {obj.currentControlScheme}");
        SetJoiningState();
        foreach (var inputDevice in obj.devices)
        {
            if (IsDevicePaired(inputDevice, obj.currentControlScheme))
            {
                pairedDevices.Remove(new KeyValuePair<InputDevice, string>(inputDevice, obj.currentControlScheme));            
            }
        }
    }

    void SetJoiningState()
    {
        if (playerInputManager.playerCount >= playerInputManager.maxPlayerCount)
        {
            playerInputManager.DisableJoining();
        }
        if (playerInputManager.playerCount < playerInputManager.maxPlayerCount)
        {
            playerInputManager.EnableJoining();
        }
    }

    void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
        playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetJoiningState();
        Debug.Log($"On player joined: n: {playerInput.gameObject.name}, s: {playerInput.currentControlScheme}");
        Debug.Log($"Joining enabled: {playerInputManager.joiningEnabled}");
    }

    void Update()
    {
        if (!playerInputManager.joiningEnabled)
        {
            return;
        }
        if (Keyboard.current[Key.LeftCtrl].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_1))
        {
            JoinPlayer(Keyboard.current, ControlScheme.KEYBOARD_1);
        }   
        if (Keyboard.current[Key.RightMeta].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_2))
        {
            JoinPlayer(Keyboard.current, ControlScheme.KEYBOARD_2);
        }   
        if (Keyboard.current[Key.Delete].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_3))
        {
            JoinPlayer(Keyboard.current, ControlScheme.KEYBOARD_3);
        }   
        if (Keyboard.current[Key.NumpadEnter].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_4))
        {
            JoinPlayer(Keyboard.current, ControlScheme.KEYBOARD_4);
        }
        
        var gamepads = Gamepad.all;
        foreach (var g in gamepads)
        {
            if (
                g[GamepadButton.South].wasPressedThisFrame &&
                !IsDevicePaired(g, ControlScheme.GAMEPAD)
            ) {
                JoinPlayer(g, ControlScheme.GAMEPAD);
            }
        } 
    }
    bool IsDevicePaired(InputDevice inputDevice, string controlScheme)
    {
        return pairedDevices.Contains(new KeyValuePair<InputDevice, string>(inputDevice, controlScheme));
    }

    void JoinPlayer(InputDevice inputDevice, string controlScheme)
    {
        pairedDevices.Add(new KeyValuePair<InputDevice, string>(inputDevice, controlScheme));

        var playerInput = playerInputManager.JoinPlayer(
            playerInputManager.playerCount,
            -1,
            controlScheme,
            inputDevice
        );
        playerInput.GetComponent<PlayerInputController>().GameStateManager = gameStateManager;
    }
}
