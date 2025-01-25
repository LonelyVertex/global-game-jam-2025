using System;
using UnityEngine;

public class GameStateMainUIController : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] GameObject welcomeScreenUI;

    public void Start()
    {
        gameStateManager.OnGameStateChanged += OnGameStateChanged;
    }

    public void OnDestroy()
    {
        gameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState obj)
    {
        if (obj == GameStateManager.GameState.INIT)
        {
            welcomeScreenUI.SetActive(true);
        }
        else
        {
            welcomeScreenUI.SetActive(false);
        }
    }
}
