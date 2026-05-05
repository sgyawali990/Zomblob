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

    [Header("VFX")]
    public float muzzleFlashScale = 1f;
    public int muzzleFlashCount = 8;

    [Header("Tracer")]
    public float tracerWidth = 0.05f;

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip reloadSound;

    public float fireVolume = 1f;
    public float reloadVolume = 1f;

    public float reloadPitchMultiplier = 1f;
}