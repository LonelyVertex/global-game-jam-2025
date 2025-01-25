using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerInputSystemManager : MonoBehaviour
{
    public enum ControlScheme {
        KEYBOARD_1,
        KEYBOARD_2,
        KEYBOARD_3,
        KEYBOARD_4,
        GAMEPAD,
    }

    public Dictionary<ControlScheme, string> ControlSchemeMapping = new ()
    {
        { ControlScheme.KEYBOARD_1, "Keyboard1" },
        { ControlScheme.KEYBOARD_2, "Keyboard2" },
        { ControlScheme.KEYBOARD_3, "Keyboard3" },
        { ControlScheme.KEYBOARD_4, "Keyboard4" },
        { ControlScheme.GAMEPAD, "Gamepad" }
    };

    public Dictionary<int, ControlScheme> PlayerIndexToControlScheme = new Dictionary<int, ControlScheme>(); 

    [SerializeField] PlayerInputManager playerInputManager;
    HashSet<KeyValuePair<InputDevice, ControlScheme>> pairedDevices = new ();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameStateManager gameStateManager;
    private bool isJoiningEnabled = false;
    

    void Start()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
        gameStateManager.OnGameStateChanged += OnGameStateChanged;
        playerInputManager.playerPrefab = playerPrefab;
        Debug.Log($"Joining enabled: {playerInputManager.joiningEnabled}");
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        SetJoiningState();
        var controlScheme = PlayerIndexToControlScheme[obj.playerIndex];
        foreach (var inputDevice in obj.devices)
        {
            if (IsDevicePaired(inputDevice, controlScheme))
            {
                pairedDevices.Remove(new KeyValuePair<InputDevice, ControlScheme>(inputDevice, controlScheme));
                PlayerIndexToControlScheme.Remove(obj.playerIndex);
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
        gameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState obj)
    {
        isJoiningEnabled = obj == GameStateManager.GameState.INIT || obj == GameStateManager.GameState.LOBBY;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetJoiningState();
        Debug.Log($"On player joined: n: {playerInput.gameObject.name}, s: {playerInput.currentControlScheme}");
        Debug.Log($"Joining enabled: {playerInputManager.joiningEnabled}");
    }

    void Update()
    {
        var gamepads = Gamepad.all;
        if (gameStateManager.State == GameStateManager.GameState.FINISHED)
        {
            var keyPressedToRestart = Keyboard.current[Key.LeftCtrl].wasPressedThisFrame ||
                Keyboard.current[Key.RightCtrl].wasPressedThisFrame ||
                Keyboard.current[Key.Delete].wasPressedThisFrame ||
                Keyboard.current[Key.NumpadEnter].wasPressedThisFrame;
            if (keyPressedToRestart)
            {
                gameStateManager.RestartGame();
            }
            foreach (var g in gamepads)
            {
                if (
                    g[GamepadButton.South].wasPressedThisFrame
                ) {
                    gameStateManager.RestartGame();
                }
            } 
            return;
        }
        if (!playerInputManager.joiningEnabled || !isJoiningEnabled)
        {
            return;
        }
        if (Keyboard.current[Key.LeftCtrl].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_1))
        {
            JoinPlayer(Keyboard.current, ControlScheme.KEYBOARD_1);
        }   
        if (Keyboard.current[Key.RightCtrl].wasPressedThisFrame && !IsDevicePaired(Keyboard.current, ControlScheme.KEYBOARD_2))
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
    bool IsDevicePaired(InputDevice inputDevice, ControlScheme controlScheme)
    {
        return pairedDevices.Contains(new KeyValuePair<InputDevice, ControlScheme>(inputDevice, controlScheme));
    }

    void JoinPlayer(InputDevice inputDevice, ControlScheme controlScheme)
    {
        var playerIndex = 0;
        if (controlScheme == ControlScheme.GAMEPAD) {
            for (var i = 0; i < 4; i++)
            {
                if (!PlayerIndexToControlScheme.ContainsKey(i))
                {
                    playerIndex = i;
                    break;
                };
            }
        }
        else
        {
            switch (controlScheme)
            {
                case ControlScheme.KEYBOARD_1:
                    playerIndex = 0;
                    break;
                case ControlScheme.KEYBOARD_2:
                    playerIndex = 1;
                    break;
                case ControlScheme.KEYBOARD_3:
                    playerIndex = 2;
                    break;
                case ControlScheme.KEYBOARD_4:
                    playerIndex = 3;
                    break;
            }
        }

        if (PlayerIndexToControlScheme.ContainsKey(playerIndex))
        {
            Debug.Log($"Controller for playerIndex: {playerIndex} is already taken by scheme: {PlayerIndexToControlScheme[playerIndex]}");
            return;
        }

        pairedDevices.Add(new KeyValuePair<InputDevice, ControlScheme>(inputDevice, controlScheme));
        PlayerIndexToControlScheme[playerIndex] = controlScheme;

        var playerInput = playerInputManager.JoinPlayer(
            playerIndex,
            -1,
            ControlSchemeMapping[controlScheme],
            inputDevice
        );
        playerInput.GetComponent<PlayerInputController>().GameStateManager = gameStateManager;
    }
}
