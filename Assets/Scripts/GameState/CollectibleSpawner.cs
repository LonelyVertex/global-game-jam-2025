using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private int maxCollectiblesPerType;
    [SerializeField] private float minSpawnTimeoutSeconds = 3.0f;
    [SerializeField] private float radiusToAvoidSpawning = 1.0f;
    
    [SerializeField] private GameStateManager gameStateManager;

    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject shotgunPrefab;
    [SerializeField] private GameObject rocketLauncherPrefab;

    [SerializeField] private bool isDebugEnabled = false;
    List<Vector3> spawnPointsWhereItDidntSpawn = new List<Vector3>();
    
    private int pistolCount = 0;
    private int shotgunCount = 0;
    private int rocketLauncherCount = 0;
    
    private float timeSinceLastSpawn = 0.0f;

    private void Start()
    {
        spawnPointsWhereItDidntSpawn.Clear();
    }

    void Update()
    {
        if (timeSinceLastSpawn <= minSpawnTimeoutSeconds)
        {
            timeSinceLastSpawn += Time.deltaTime;
            return;
        }
        if (gameStateManager.State == GameStateManager.GameState.RUNNING)
        {
            var gunType = Random.Range(0, 3);
            if (gunType == 0 && pistolCount >= maxCollectiblesPerType)
            {
                return;
            }
            if (gunType == 1 && shotgunCount >= maxCollectiblesPerType)
            {
                return;
            }
            if (gunType == 2 && rocketLauncherCount >= maxCollectiblesPerType)
            {
                return;
            }

            TrySpawnWeapon(gunType);
        }
    }
    
    void TrySpawnWeapon(int gunType)
    {
        var x = Random.Range(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2);
        var y = Random.Range(transform.position.y - transform.localScale.y / 2, transform.position.y + transform.localScale.y / 2);
        var anythingAround = Physics2D.OverlapCircle(new Vector2(x, y), radiusToAvoidSpawning);
        if (anythingAround)
        {
            if (isDebugEnabled)
            {
                spawnPointsWhereItDidntSpawn.Add(new Vector3(x, y, 0));
            }
            return;
        }

        GameObject prefabGO;
        switch (gunType)
        {
            case 0:
                prefabGO = pistolPrefab;
                break;
            case 1:
                prefabGO = shotgunPrefab;
                break;
            case 2:
                prefabGO = rocketLauncherPrefab;
                break;
            default:
                Debug.LogError($"Unsupported gun type = {gunType}");
                return;
        }

        var collectibleGO = Instantiate(prefabGO, new Vector3(x, y, 0), Quaternion.identity);
        collectibleGO.GetComponent<CollectibleWeapon>().OnSpawned += OnCollectibleSpawned;
        timeSinceLastSpawn = 0.0f;
    }

    private void OnCollectibleSpawned(CollectibleWeapon obj)
    {
        obj.OnCollected += OnCollected;
        obj.OnSpawned -= OnCollectibleSpawned;
        switch (obj.weaponType)
        {
            case PlayerController.WeaponType.Pistol:
                pistolCount++;
                break;
            case PlayerController.WeaponType.Shotgun:
                shotgunCount++;
                break;
            case PlayerController.WeaponType.RocketLauncher:
                rocketLauncherCount++;
                break;
        }
    }

    private void OnCollected(CollectibleWeapon obj)
    {
        obj.OnCollected -= OnCollected;
        switch (obj.weaponType)
        {
            case PlayerController.WeaponType.Pistol:
                pistolCount--;
                break;
            case PlayerController.WeaponType.Shotgun:
                shotgunCount--;
                break;
            case PlayerController.WeaponType.RocketLauncher:
                rocketLauncherCount--;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        if (isDebugEnabled) {
            Gizmos.color = Color.yellow;
            foreach (var vector3 in spawnPointsWhereItDidntSpawn)
            {
                Gizmos.DrawWireSphere(vector3, radiusToAvoidSpawning);
            }
        }
    }
}
