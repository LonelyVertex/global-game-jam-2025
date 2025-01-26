using System;
using UnityEngine;

public class CollectibleWeapon : MonoBehaviour
{

    public PlayerController.WeaponType weaponType;
    GameStateManager gameStateManager;
    public int ammo = 10;

    public bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("CollectibleWeapon.OnTriggerEnter2D");
        Debug.Log("other.tag: " + other.tag);
        Debug.Log("other.name: " + other.name);
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;
            PlayerController player = other.GetComponentInParent<PlayerController>();
            player.SetAmmo(weaponType, ammo);
            player.SetWeapon(weaponType);
            DestroyCollectible();
        }
    }

    private void SetGameStateManager(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        gameStateManager.OnGameStateChanged += OnGameStateChange;
    }

    private void OnGameStateChange(GameStateManager.GameState obj)
    {
        if (obj == GameStateManager.GameState.INIT)
        {
            DestroyCollectible();
        }
    }

    private void DestroyCollectible()
    {
        OnCollected?.Invoke(this);
        Destroy(gameObject);
    }

    private void Start()
    {
        OnSpawned?.Invoke(this);
    }

    public event Action<CollectibleWeapon> OnSpawned;
    public event Action<CollectibleWeapon> OnCollected;
}
