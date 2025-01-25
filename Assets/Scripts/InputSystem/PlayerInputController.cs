using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    private bool _isPlayerReady = false;
    private bool IsPlayerReady
    {
        get => _isPlayerReady;
        set
        {
            GameStateManager.SetPlayerReadyState(playerInput, value);
            _isPlayerReady = value;
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
        GameStateManager.PlayerDisconnected(playerInput);
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
        playerGO = Instantiate(playerPrefab);
        playerController = playerGO.GetComponent<PlayerController>();
    }

    public void Update()
    {
        if (GameStateManager.State == GameStateManager.GameState.RUNNING) {
            var fwd = forward.ReadValue<float>();
            var bck = backward.ReadValue<float>();
            var lft = left.ReadValue<float>();
            var rgt = right.ReadValue<float>();
            var sht = shoot.ReadValue<float>();
            var dv = dive.ReadValue<float>();
            var j = jump.ReadValue<float>();
            
            playerController.SetInputVector(new Vector4(fwd, bck, lft, rgt));
        }
    }
}