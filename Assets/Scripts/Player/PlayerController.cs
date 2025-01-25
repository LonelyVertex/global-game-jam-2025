using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public enum WeaponType
    {
        Pistol,
        RocketLauncher,
        Hands
    }

    public float acceleration = 0.3f;
    public float rotationSpeed = 0.1f;

    public Transform projectileSpawnPoint;

    public GameObject projectilePrefab;

    public GameObject gunProjectilePrefab;
    public GameObject rocketProjectilePrefab;

    public GameObject pistolWeaponPrefab;
    public GameObject rocketLauncherWeaponPrefab;

    public WeaponType currentWeapon = WeaponType.Pistol;

    private Dictionary<WeaponType, int> weaponAmmo = new Dictionary<WeaponType, int>
    {
        {WeaponType.RocketLauncher, 0},
        {WeaponType.Pistol, 0},
        {WeaponType.Hands, 0},
    };

    /// <summary>
    /// (acceleration, deceleartion, rotationLeft, rotationRight)
    /// </summary>
    private Vector4 inputVector = Vector4.zero;

    void FixedUpdate()
    {
        ApplyInputVector();
    }

    void Update()
    {
        UpdateWeapon();
    }

    private void UpdateWeapon()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                pistolWeaponPrefab.SetActive(true);
                rocketLauncherWeaponPrefab.SetActive(false);
                break;
            case WeaponType.RocketLauncher:
                pistolWeaponPrefab.SetActive(false);
                rocketLauncherWeaponPrefab.SetActive(true);
                break;
            case WeaponType.Hands:
                pistolWeaponPrefab.SetActive(false);
                rocketLauncherWeaponPrefab.SetActive(false);
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
        if (weaponAmmo[currentWeapon] > 0)
        {
            weaponAmmo[currentWeapon]--;

            switch (currentWeapon)
            {
                case WeaponType.Pistol:
                    Instantiate(gunProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    break;
                case WeaponType.RocketLauncher:
                    Instantiate(rocketProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    break;
            }
        } else {
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
        Debug.Log("Accelerated");
        rb.AddForce(rb.transform.up * acceleration, ForceMode2D.Force);
    }

    private void Decelerate()
    {
        Debug.Log("Decelerated");
        rb.AddForce(-rb.transform.up * acceleration, ForceMode2D.Force);
    }
    private void RotateLeft()
    {
        Debug.Log("Rotated Left");
        rb.AddTorque(rotationSpeed, ForceMode2D.Force);
    }

    private void RotateRight()
    {
        Debug.Log("Rotated Right");
        rb.AddTorque(-rotationSpeed, ForceMode2D.Force);
    }
}
