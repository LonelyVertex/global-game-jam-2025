using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerVisualController visualController;
    public Rigidbody2D rb;
    public event Action OnScoreChanged;
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

    public WeaponType currentWeapon = WeaponType.Pistol;

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

    private bool underwater = false;

    private float divingTimeLimit = 2.0f;
    private float divingCooldown = 4.0f;
    private float divingTime = 0.0f;
    private float divingCooldownTime = 0.0f;
    private bool divingEnabled = true;

    void Start()
    {
        visualController.SetType(underwater);
    }

    void FixedUpdate()
    {
        ApplyInputVector();
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
        weaponAmmo[weapon] = ammo;
        Debug.Log("Set ammo for " + weapon + " to " + ammo);
    }

    public void Fire()
    {
        if (underwater)
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
                    break;
                case WeaponType.RocketLauncher:
                    Debug.Log("Firing Rocket Launcher");
                    var rocketInstance = Instantiate(rocketProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    rocketInstance.GetComponent<ProjectileController>().playerController = this;
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
                    break;

            }
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
        if (inputVector.x > 0)
        {
            Accelerate();
        }

        if (inputVector.y > 0)
        {
            Decelerate();
        }

        if (inputVector.z > 0)
        {
            RotateLeft();
        }

        if (inputVector.w > 0)
        {
            RotateRight();
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

    public void IncremeantScore()
    {
        score++;
        Debug.Log("Score: " + score);
        OnScoreChanged?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }

    public void SetUnderwater(bool underwater)
    {
        if (underwater != this.underwater)
        {

            if (!underwater)
            {
                divingCooldownTime = 0.0f;
                divingEnabled = false;
                this.underwater = underwater;
                divingTime = 0.0f;
                visualController.SetType(underwater);
            }
            else
            {
                if (divingEnabled)
                {
                    divingEnabled = false;
                    divingTime = 0.0f;
                    this.underwater = underwater;
                    visualController.SetType(underwater);
                }
            }
        }

    }

    public bool IsUnderwater()
    {
        return underwater;
    }
}
