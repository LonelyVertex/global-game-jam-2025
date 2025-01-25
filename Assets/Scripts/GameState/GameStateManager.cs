using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager playerInputManager;
    
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>();

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

    public void SetPlayerReadyState(PlayerInput obj, bool isReady)
    {
        playerReadyStates[obj.playerIndex] = isReady;
    }

    public void PlayerDisconnected(PlayerInput obj)
    {
        playerReadyStates.Remove(obj.playerIndex);
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
