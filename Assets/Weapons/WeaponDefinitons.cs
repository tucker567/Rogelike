using UnityEngine;

[CreateAssetMenu(menuName="Weapon")]
public class WeaponDefinitons : MonoBehaviour
{
    public string weaponName;
    public Sprite Icon;
    public Sprite sprite; // what the player holds
    public GameObject bulletPrefab;
    public float damage;
    public float knockback;
    public float fireRate;

    [TextArea] public string description;
}
