using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    private bool _isPlayerReady = false;
    public bool IsPlayerReady
    {
        get => _isPlayerReady;
        private set
        {
            _isPlayerReady = value;
            OnReadyStateChange?.Invoke(value);
        }
    }

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
    public GameStateManager GameStateManager { private get; set; }
    private GameObject playerGO;
    public event Action<bool> OnReadyStateChange;

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
        
        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        IsPlayerReady = false;
    }

    public void OnDestroy()
    {
        Destroy(playerGO);
        GameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState gameState)
    {
        switch (gameState) {
            case GameStateManager.GameState.INIT:
                Destroy(this.gameObject);
                break;
            case GameStateManager.GameState.LOBBY:
                IsPlayerReady = false;
                if (playerGO != null) {
                    Destroy(playerGO);
                }

                break;
            case GameStateManager.GameState.RUNNING:
                SpawnPlayer();
                break;
            case GameStateManager.GameState.FINISHED:
                IsPlayerReady = false;
                break;
        }
    }

    private void SpawnPlayer()
    {
        var spawnPoint = GameStateManager.GetSpawnPoint(playerInput.playerIndex);
        playerGO = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerController = playerGO.GetComponent<PlayerController>();
        var visuals = playerGO.GetComponent<PlayerVisualController>();
        visuals.SetVisuals(playerInput.playerIndex);
        playerController.OnScoreChanged += OnPlayerScoreChanged;
        playerController.OnDeathCountChanged += OnPlayerDeathCountChanged;
    }

    private void OnPlayerScoreChanged()
    {
        var newScore = playerController.GetScore();
        GameStateManager.PlayerUIControllers[playerInput.playerIndex].SetKillsValue(newScore);
        if (newScore >= GameStateManager.ScoreToFinish)
        {
            GameStateManager.FinishGame();
            GameStateManager.PlayerUIControllers[playerInput.playerIndex].SetWinner();
        }
    }

    private void OnPlayerDeathCountChanged()
    {
        var newDeathCount = playerController.GetDeathCount();
        GameStateManager.PlayerUIControllers[playerInput.playerIndex].SetDeathsValue(newDeathCount);
    }

    public void Update()
    {
        if (GameStateManager.State == GameStateManager.GameState.LOBBY)
        {
            var sht = shoot.WasPressedThisFrame();
            if (sht) {
                IsPlayerReady = !IsPlayerReady;
            }
        }
        if (GameStateManager.State == GameStateManager.GameState.RUNNING) {
            var fwd = forward.ReadValue<float>();
            var bck = backward.ReadValue<float>();
            var lft = left.ReadValue<float>();
            var rgt = right.ReadValue<float>();
            var sht = shoot.WasPressedThisFrame();
            if (sht)
            {
                playerController.Fire();
            }
            var dv = dive.ReadValue<float>();
            playerController.SetUnderwater(dv > 0);
            var j = jump.ReadValue<float>();
            
            playerController.SetInputVector(new Vector4(fwd, bck, lft, rgt));
        }
    }
}
