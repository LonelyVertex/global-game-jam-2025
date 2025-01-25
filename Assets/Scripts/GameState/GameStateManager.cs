using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager playerInputManager;
    
    private HashSet<PlayerInputController> playerInputControllers = new HashSet<PlayerInputController>();
    
    public enum GameState
    {
        INIT,
        LOBBY,
        RUNNING,
        FINISHED,
    }

    public event Action<GameState> OnGameStateChanged;

    private GameState _state = GameState.INIT;
    public GameState State
    {
        get => _state;
        private set
        {
            _state = value;
            OnGameStateChanged?.Invoke(_state);
        }
    }

    public void Start()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
        State = GameState.INIT;
    }

    public void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
        playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput obj)
    {
        var playerInputController = obj.GetComponent<PlayerInputController>();
        playerInputControllers.Add(playerInputController);
        playerInputController.OnReadyStateChange += OnPlayerReadyStateChange;
        if (State == GameState.INIT)
        {
            State = GameState.LOBBY;
        }
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        var playerInputController = obj.GetComponent<PlayerInputController>();
        playerInputControllers.Remove(playerInputController);
        playerInputController.OnReadyStateChange -= OnPlayerReadyStateChange;
        if (playerInputManager.playerCount == 0)
        {
            State = GameState.INIT;
        }
    }

    private void OnPlayerReadyStateChange(bool isPlayerReady)
    {
        if (isPlayerReady)
        {
            if (playerInputControllers.All(playerInputController => playerInputController.IsPlayerReady))
            {
                State = GameState.RUNNING;
            }
        }
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current[Key.F5].wasPressedThisFrame)
        {
            StartLobby();
        }
        if (Keyboard.current[Key.F2].wasPressedThisFrame)
        {
            RestartGame();
        }
        if (Keyboard.current[Key.F3].wasPressedThisFrame)
        {
            StartGameplay();
        }
        if (Keyboard.current[Key.F4].wasPressedThisFrame)
        {
            FinishGame();
        }
    }
#endif

    private void RestartGame()
    {
        State = GameState.INIT;
    }

    private void StartLobby()
    {
        State = GameState.LOBBY;
    }

    private void StartGameplay()
    {
        State = GameState.RUNNING;
    }

    private void FinishGame()
    {
        State = GameState.FINISHED;
    }
}
