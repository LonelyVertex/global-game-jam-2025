using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private PlayerGameplayStatUIController killsCounter;
    [SerializeField] private PlayerGameplayStatUIController deathsCounter;
    [SerializeField] private GameObject joinHintUI;
    [SerializeField] private GameObject readyHintUI;
    [SerializeField] private GameObject readyUI;
    [SerializeField] private GameObject gamePlayUI;
    [SerializeField] private GameObject finishedGameUI;
    [SerializeField] private int playerIndex;
    
    private PlayerInputController playerInputController;
    private GameObject[] allUIObjects;


    private void Start()
    {
        allUIObjects = new[]
        {
            joinHintUI,
            readyHintUI,
            readyUI,
            gamePlayUI,
            finishedGameUI
        };
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
        gameStateManager.OnGameStateChanged += OnGameStateChanged;
        SetInitUI();
    }

    private void OnGameStateChanged(GameStateManager.GameState obj)
    {
        if (obj == GameStateManager.GameState.RUNNING)
        {
            SetGameplayUI();
        }

        if (obj == GameStateManager.GameState.FINISHED)
        {
            SetFinishedGameUI();
        }
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        if (obj.playerIndex != playerIndex)
        {
            return;
        }
        SetInitUI();
        playerInputController.OnReadyStateChange -= OnPlayerReadyStateChange;
        playerInputController = null;
    }

    private void OnPlayerJoined(PlayerInput obj)
    {
        if (obj.playerIndex != playerIndex)
        {
            return;
        }
        playerInputController = obj.GetComponent<PlayerInputController>();
        playerInputController.OnReadyStateChange += OnPlayerReadyStateChange;
        SetJoinedUI();
    }

    private void OnPlayerReadyStateChange(bool isReady)
    {
        if (gameStateManager.State == GameStateManager.GameState.INIT ||
            gameStateManager.State == GameStateManager.GameState.LOBBY)
        {
            if (isReady)
            {
                SetReadyUI();
            }
            else
            {
                SetJoinedUI();
            }
        }
    }

    private void HideAllUI()
    {
        foreach (var uiObject in allUIObjects)
        {
            uiObject.SetActive(false);
        }
    }

    public void SetInitUI()
    {
        HideAllUI();
        Reset();
        joinHintUI.SetActive(true);
    }

    public void SetJoinedUI()
    {
        HideAllUI();
        readyHintUI.SetActive(true);
    }
    
    public void SetReadyUI()
    {
        HideAllUI();
        readyUI.SetActive(true);
    }
    
    public void SetGameplayUI()
    {
        HideAllUI();
        gamePlayUI.SetActive(true);
    }
    
    public void SetFinishedGameUI()
    {
        HideAllUI();
        finishedGameUI.SetActive(true);
    }

    public void Reset()
    {
        killsCounter.SetScoreValue(0);
        deathsCounter.SetScoreValue(0);
    }

    public void SetKillsValue(int value)
    {
        killsCounter.SetScoreValue(value);
    }

    public void SetDeathsValue(int value)
    {
        deathsCounter.SetScoreValue(value);
    }
}
