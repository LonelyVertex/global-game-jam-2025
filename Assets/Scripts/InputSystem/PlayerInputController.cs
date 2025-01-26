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
                IsPlayerReady = false;
                break;
            case GameStateManager.GameState.LOBBY:
                IsPlayerReady = false;
                if (playerGO != null) {
                    Destroy(playerGO);
                }
                break;
            case GameStateManager.GameState.RUNNING:
                break;
            case GameStateManager.GameState.FINISHED:
                IsPlayerReady = false;
                break;
        }
    }

    public void SetGameObject(GameObject gameObject)
    {
        playerGO = gameObject;
        playerController = playerGO.GetComponent<PlayerController>();
    }

    public void Update()
    {
        var sht = shoot.WasPressedThisFrame();
        if (GameStateManager.State == GameStateManager.GameState.LOBBY)
        {
            if (sht) {
                IsPlayerReady = !IsPlayerReady;
            }
            return;
        }

        if (playerController == null)
        {
            return;
        }
        var fwd = forward.ReadValue<float>();
        var bck = backward.ReadValue<float>();
        var lft = left.ReadValue<float>();
        var rgt = right.ReadValue<float>();
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
