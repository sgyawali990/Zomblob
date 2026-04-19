using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [Header("Identity")]
    public WeaponType weaponType;
    public string displayName;

    [Header("Type")]
    public bool isGun;
    public bool isMelee;

    [Header("Ammo")]
    public AmmoType ammoType;
    public int magazineSize;
    public int maxReserveAmmo;

    [Header("Combat")]
    public FireMode fireMode;
    public float damage;
    public float fireRate;
    public float range;

    [Header("Reload")]
    public ReloadType reloadType;
    public float reloadTime;

    [Header("Spread / Recoil")]
    public float spread;
    public float recoil;

    [Header("Tracer")]
    public float tracerWidth = 0.05f;
}