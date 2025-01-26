using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager playerInputManager;
    [SerializeField] public int ScoreToFinish;

    private HashSet<int> localPlayerIndices = new HashSet<int>();
    private Dictionary<int, PlayerInputController> playerInputControllers = new Dictionary<int, PlayerInputController>();
    [SerializeField]
    public List<PlayerUIController> PlayerUIControllers = new List<PlayerUIController>();
    [SerializeField]
    public List<DuckSpawner> Spawners = new List<DuckSpawner>();
    [SerializeField]
    public GameObject AIBotPrefab;
    [SerializeField]
    public GameObject LocalPlayerPrefab;
    [SerializeField]
    public bool areAIBotsEnabled = true;

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
            if (value == GameState.RUNNING)
            {
                SpawnPlayers();
            }
        }
    }

    public void Start()
    {
        areAIBotsEnabled = GameSettingsController.GetAIBotsEnabled();
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

    public void SpawnPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            var spawnPoint = GetSpawnPoint(i);
            var playerPrefab = playerInputControllers.ContainsKey(i) ? LocalPlayerPrefab : AIBotPrefab;
            if (!playerInputControllers.ContainsKey(i) && !areAIBotsEnabled)
            {
                continue;
            }

            var playerGO = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                var playerController = playerGO.GetComponentInChildren<PlayerController>();
                var visuals = playerGO.GetComponentInChildren<PlayerVisualController>();
                visuals.SetVisuals(i);
                playerController.PlayerIndex = i;
                playerController.gameStateManager = this;
                playerController.OnScoreChanged += OnPlayerScoreChanged;
                var pic = playerInputControllers.ContainsKey(i) ? playerInputControllers[i] : null;
                pic?.SetGameObject(playerGO);
        }
    }

    private void OnPlayerScoreChanged(PlayerController player)
    {
        var score = player.GetScore();
        PlayerUIControllers[player.PlayerIndex].SetKillsValue(score);
        PlayerUIControllers[player.PlayerIndex].SetDeathsValue(player.GetDeathCount());

        if (score >= ScoreToFinish) {
            FinishGame();
            PlayerUIControllers[player.PlayerIndex].SetWinner();
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
        //Destroy all projectiles
        var projectiles = FindObjectsOfType<ProjectileController>();
        Array.ForEach(projectiles, p => p.DestroyProjectile());
    }

    public Transform GetSpawnPoint(int playerIndex)
    {
        return Spawners.Find(spawner => spawner.PlayerIndex == playerIndex).transform;
    }
}
