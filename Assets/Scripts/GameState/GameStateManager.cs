using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager playerInputManager;
    [SerializeField] public int ScoreToFinish;

    private Dictionary<int, PlayerInputController> playerInputControllers = new Dictionary<int, PlayerInputController>();
    [SerializeField]
    public List<PlayerUIController> PlayerUIControllers = new List<PlayerUIController>();
    [SerializeField]
    public List<DuckSpawner> Spawners = new List<DuckSpawner>();

    private FadeController _fadeController;

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
        _fadeController = FindFirstObjectByType<FadeController>();
        if (_fadeController != null)
        {
            _fadeController.FadeIn();
        }

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
        playerInputController.OnReadyStateChange += OnPlayerReadyStateChange;
        playerInputControllers.Add(obj.playerIndex, playerInputController);
        if (State == GameState.INIT)
        {
            State = GameState.LOBBY;
        }
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        var playerInputController = playerInputControllers[obj.playerIndex];
        playerInputController.OnReadyStateChange -= OnPlayerReadyStateChange;
        if (playerInputManager.playerCount == 0)
        {
            State = GameState.INIT;
        }
        playerInputControllers.Remove(obj.playerIndex);
    }

    private void OnPlayerReadyStateChange(bool isPlayerReady)
    {
        if (isPlayerReady)
        {
            if (playerInputControllers.All(pair => pair.Value.IsPlayerReady))
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

    public void RestartGame()
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

    public void FinishGame()
    {
        State = GameState.FINISHED;
    }

    public Transform GetSpawnPoint(int playerIndex)
    {
        return Spawners.Find(spawner => spawner.PlayerIndex == playerIndex).transform;
    }
}
