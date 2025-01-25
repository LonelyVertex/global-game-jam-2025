using UnityEngine;

public class CollectibleWeapon : MonoBehaviour
{

    public PlayerController.WeaponType weaponType;
    public int ammo = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("CollectibleWeapon.OnTriggerEnter2D");
        Debug.Log("other.tag: " + other.tag);
        Debug.Log("other.name: " + other.name);
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            player.SetAmmo(weaponType, ammo);
            player.SetWeapon(weaponType);
            Destroy(gameObject);
        }
    }
}
