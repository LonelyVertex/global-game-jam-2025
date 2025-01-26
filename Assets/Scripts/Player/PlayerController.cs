using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public PlayerVisualController visualController;
    public Rigidbody2D rb;
    public PlayerAudioController audioController;

    public event Action OnScoreChanged;
    public event Action OnDeathCountChanged;
    public enum WeaponType
    {
        Pistol,
        RocketLauncher,
        Shotgun,
        Hands
    }

    public float acceleration = 0.3f;
    public float rotationSpeed = 0.1f;

    public Transform projectileSpawnPoint;

    public GameObject projectilePrefab;

    public GameObject gunProjectilePrefab;
    public GameObject rocketProjectilePrefab;
    public GameObject shotgunProjectilePrefab;

    public GameObject pistolWeaponPrefab;
    public GameObject rocketLauncherWeaponPrefab;
    public GameObject shotgunWeaponPrefab;

    [Space]
    public Transform playerTrailParentTransform;
    public Transform playerTrailTransform;
    public ParticleSystem particleSystem;

    public WeaponType currentWeapon = WeaponType.Pistol;

    public float underwaterDetectRadius;
    public LayerMask waterLayer;

    private Dictionary<WeaponType, int> weaponAmmo = new Dictionary<WeaponType, int>
    {
        {WeaponType.RocketLauncher, 0},
        {WeaponType.Shotgun, 0},
        {WeaponType.Pistol, 0},
        {WeaponType.Hands, 0},
    };

    /// <summary>
    /// (acceleration, deceleartion, rotationLeft, rotationRight)
    /// </summary>
    private Vector4 inputVector = Vector4.zero;

    private int score = 0;
    private int deathCount = 0;

    private bool underwater = false;

    private float divingTimeLimit = 2.0f;
    private float divingCooldown = 4.0f;
    private float divingTime = 0.0f;
    private float divingCooldownTime = 0.0f;
    private bool divingEnabled = true;

    public bool killed = false;
    private float spawnDownTime = 3.0f;
    private float spawnTime = 0.0f;


    private List<GameObject> spawnPoints = new List<GameObject>();
    float fireCooldown = 1f;
    float lastFireTime = -20f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, underwaterDetectRadius);
    }

    void Start()
    {
        visualController.SetType(underwater);
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint"));
    }

    void FixedUpdate()
    {
        ApplyInputVector();
        if (killed)
        {
            spawnTime += Time.fixedDeltaTime;
            if (spawnTime >= spawnDownTime)
            {
                Respawn();
            }
        }
    }

    void Update()
    {
        UpdateWeapon();
        if (underwater)
        {
            divingTime += Time.deltaTime;
            if (divingTime >= divingTimeLimit)
            {
                SetUnderwater(false);
            }
        }
        else
        {
            if (!divingEnabled)
            {
                divingCooldownTime += Time.deltaTime;
                if (divingCooldownTime >= divingCooldown)
                {
                    divingEnabled = true;
                }
            }
        }
    }

    private void UpdateWeapon()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                pistolWeaponPrefab.SetActive(true);
                rocketLauncherWeaponPrefab.SetActive(false);
                shotgunWeaponPrefab.SetActive(false);
                break;
            case WeaponType.RocketLauncher:
                pistolWeaponPrefab.SetActive(false);
                rocketLauncherWeaponPrefab.SetActive(true);
                shotgunWeaponPrefab.SetActive(false);
                break;
            case WeaponType.Shotgun:
                pistolWeaponPrefab.SetActive(false);
                rocketLauncherWeaponPrefab.SetActive(false);
                shotgunWeaponPrefab.SetActive(true);
                break;
            case WeaponType.Hands:
                pistolWeaponPrefab.SetActive(false);
                rocketLauncherWeaponPrefab.SetActive(false);
                shotgunWeaponPrefab.SetActive(false);
                break;
        }
    }

    public void SetInputVector(Vector4 input)
    {
        inputVector = input;
    }

    public void SetWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;

        audioController.SetNextWeapon();
    }

    public void SetNextWeapon()
    {
        foreach (var weapon in weaponAmmo)
        {
            if (weapon.Value > 0)
            {
                SetWeapon(weapon.Key);
                return;
            }
        }
        SetWeapon(WeaponType.Hands);
    }

    public void SetAmmo(WeaponType weapon, int ammo)
    {
        weaponAmmo[weapon] += ammo;
        Debug.Log("Set ammo for " + weapon + " to " + ammo);

        audioController.CollectiblePickup();
    }

    public void Fire()
    {
        if (killed)
        {
            return;
        }

        if (underwater)
        {
            return;
        }

        if (lastFireTime + fireCooldown > Time.time)
        {
            return;
        }

        Debug.Log("Fire");
        if (weaponAmmo[currentWeapon] > 0)
        {
            weaponAmmo[currentWeapon]--;
            switch (currentWeapon)
            {
                case WeaponType.Pistol:
                    Debug.Log("Firing Pistol");
                    var gunInstance = Instantiate(gunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    gunInstance.GetComponent<ProjectileController>().playerController = this;
                    audioController.ShootPistol();
                    break;
                case WeaponType.RocketLauncher:
                    Debug.Log("Firing Rocket Launcher");
                    var rocketInstance = Instantiate(rocketProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    rocketInstance.GetComponent<ProjectileController>().playerController = this;
                    audioController.ShootBazooka();
                    break;
                case WeaponType.Shotgun:
                    Debug.Log("Firing Shotgun");
                    var shotgunInstance1 = Instantiate(shotgunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    shotgunInstance1.GetComponent<ProjectileController>().playerController = this;
                    var shotgunInstance2 = Instantiate(shotgunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation * Quaternion.Euler(0, 0, 5));
                    shotgunInstance2.GetComponent<ProjectileController>().playerController = this;
                    var shotgunInstance3 = Instantiate(shotgunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation * Quaternion.Euler(0, 0, 10));
                    shotgunInstance3.GetComponent<ProjectileController>().playerController = this;
                    var shotgunInstance4 = Instantiate(shotgunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation * Quaternion.Euler(0, 0, -10));
                    shotgunInstance4.GetComponent<ProjectileController>().playerController = this;
                    var shotgunInstance5 = Instantiate(shotgunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation * Quaternion.Euler(0, 0, -5));
                    shotgunInstance5.GetComponent<ProjectileController>().playerController = this;
                    audioController.ShootShotgun();
                    break;

            }

            lastFireTime = Time.time;

            if (weaponAmmo[currentWeapon] == 0)
            {
                SetNextWeapon();
            }
        }
        else
        {
            SetNextWeapon();
        }
    }

    private void ApplyInputVector()
    {
        if (killed)
        {
            return;
        }

        var anyInput = false;
        if (inputVector.x > 0)
        {
            Accelerate();
            anyInput = true;
        }

        if (inputVector.y > 0)
        {
            Decelerate();
            anyInput = true;
        }

        if (inputVector.z > 0)
        {
            RotateLeft();
            anyInput = true;
        }

        if (inputVector.w > 0)
        {
            RotateRight();
            anyInput = true;
        }

        if (anyInput && Random.Range(0.0f, 1000.0f) < 10.0f)
        {
            audioController.Movement();
        }
    }

    private void Accelerate()
    {
        //Debug.Log("Accelerated");
        rb.AddForce(rb.transform.up * acceleration, ForceMode2D.Force);
    }

    private void Decelerate()
    {
        //Debug.Log("Decelerated");
        rb.AddForce(-rb.transform.up * acceleration, ForceMode2D.Force);
    }
    private void RotateLeft()
    {
        //Debug.Log("Rotated Left");
        rb.AddTorque(rotationSpeed, ForceMode2D.Force);
    }

    private void RotateRight()
    {
        //Debug.Log("Rotated Right");
        rb.AddTorque(-rotationSpeed, ForceMode2D.Force);
    }

    public void IncrementScore()
    {
        score++;
        Debug.Log("Score: " + score);
        OnScoreChanged?.Invoke();
    }

    public void ReduceScore()
    {
        score = Math.Max(0, score - 1);
        Debug.Log("Score: " + score);
        OnScoreChanged?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }

    public int GetDeathCount()
    {
        return deathCount;
    }

    public void SetUnderwater(bool underwater)
    {
        if (killed)
        {
            return;
        }
        if (underwater != this.underwater)
        {

            if (!underwater)
            {
                if (CanRise())
                {
                    divingCooldownTime = 0.0f;
                    divingEnabled = false;
                    this.underwater = underwater;
                    divingTime = 0.0f;
                    visualController.SetType(underwater);
                    audioController.Rise();
                }
            }
            else
            {
                if (divingEnabled)
                {
                    divingEnabled = false;
                    divingTime = 0.0f;
                    this.underwater = underwater;
                    visualController.SetType(underwater);
                    audioController.Dive();
                }
            }
        }

    }

    bool CanRise()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, underwaterDetectRadius, waterLayer);
        return colliders.All(col => col.GetComponent<CollectibleWeapon>());
    }

    public bool IsUnderwater()
    {
        return underwater;
    }

    public void Kill()
    {
        playerTrailTransform.SetParent(null, worldPositionStays: true);
        particleSystem.Stop();

        Debug.Log("Player Killed");
        gameObject.GetComponentInChildren<PlayerBody>(true).gameObject.SetActive(false);
        killed = true;
        deathCount++;
        InitPlayerState();
        OnDeathCountChanged?.Invoke();

        audioController.Death();
    }

    public void Respawn()
    {
        var spawnLocation = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        var colliders = Physics2D.OverlapCircleAll(spawnLocation.transform.position, underwaterDetectRadius, waterLayer);
        if (colliders.All(col => col.GetComponent<CollectibleWeapon>()))
        {
            Debug.Log("Player Respawned");
            killed = false;
            InitPlayerState();
            rb.position = spawnLocation.transform.position;
            gameObject.GetComponentInChildren<PlayerBody>(true).gameObject.SetActive(true);

            playerTrailTransform.SetParent(playerTrailParentTransform, worldPositionStays: false);
            playerTrailTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            particleSystem.Play();
        }
    }

    public void InitPlayerState()
    {
        weaponAmmo[WeaponType.Pistol] = 0;
        weaponAmmo[WeaponType.RocketLauncher] = 0;
        weaponAmmo[WeaponType.Shotgun] = 0;
        weaponAmmo[WeaponType.Hands] = 0;
        spawnTime = 0.0f;
        divingTime = 0.0f;
        divingEnabled = true;
        SetUnderwater(false);
        SetWeapon(WeaponType.Hands);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0.0f;
    }
}
