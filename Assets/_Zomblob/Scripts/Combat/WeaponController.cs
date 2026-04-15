using UnityEngine;
using System.Collections; // Added for Coroutines

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public FireInput fireInput;
    public PlayerController playerController;
    private PlayerInventory inventory;
    private WeaponData weaponData;
    [SerializeField] private LineRenderer line;
    public Transform firePoint;

    [Header("Settings (Overridden by WeaponData if found)")]
    public float fireRate = 5f;
    public float range = 100f;
    public float damage = 10f;

    private float nextFireTime;
    private bool isBursting = false; 

    void Start()
    {
        // Find dependencies
        if (fireInput == null) fireInput = Object.FindFirstObjectByType<FireInput>();
        if (playerController == null) playerController = Object.FindFirstObjectByType<PlayerController>();

        inventory = Object.FindFirstObjectByType<PlayerInventory>();
        weaponData = GetComponent<WeaponData>();

        // Sync stats from WeaponData asset
        if (weaponData != null)
        {
            fireRate = weaponData.fireRate;
            range = weaponData.range;
            damage = weaponData.damage;
        }

        if (line != null)
        {
            line.positionCount = 2;
        }
    }

    void Update()
    {
        if (fireInput == null || firePoint == null || inventory == null) return;

        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.forward;

        // Update visual aim line
        if (dir.sqrMagnitude > 0.001f && line != null)
        {
            line.SetPosition(0, origin);
            line.SetPosition(1, origin + dir * range);
        }

        // Fire Logic
        bool canFire = Time.time >= nextFireTime;

        if (weaponData.fireMode == FireMode.FullAuto)
        {
            if (Input.GetMouseButton(0) && canFire)
            {
                Fire(origin, dir);
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
        else if (weaponData.fireMode == FireMode.SemiAuto)
        {
            if (Input.GetMouseButtonDown(0) && canFire)
            {
                Fire(origin, dir);
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
        // Burst fire logic
        else if (weaponData.fireMode == FireMode.Burst)
        {
            if (Input.GetMouseButtonDown(0) && canFire && !isBursting)
            {
                StartCoroutine(BurstFire());
            }
        }

        // Reload Logic
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    // BURST COROUTINE
    IEnumerator BurstFire()
    {
        isBursting = true;
        int shots = 3;

        for (int i = 0; i < shots; i++)
        {
            if (inventory.GetCurrentAmmo() <= 0) break;

            Vector3 currentOrigin = firePoint.position;
            Vector3 currentDir = firePoint.forward;

            Fire(currentOrigin, currentDir);

            yield return new WaitForSeconds(1f / fireRate);
        }

        nextFireTime = Time.time + (1f / fireRate);
        isBursting = false;
    }

    void Fire(Vector3 origin, Vector3 dir)
    {
        int currentAmmo = inventory.GetCurrentAmmo();

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        if (dir.sqrMagnitude < 0.001f) return;

        // Subtract ammo and save it back to the Inventory slot
        currentAmmo--;
        inventory.SetCurrentAmmo(currentAmmo);

        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }

            if (line != null)
            {
                line.SetPosition(0, origin);
                line.SetPosition(1, hit.point);
            }
        }
        else
        {
            if (line != null)
            {
                line.SetPosition(0, origin);
                line.SetPosition(1, origin + dir * range);
            }
        }
    }

    void Reload()
    {
        if (weaponData == null || inventory == null) return;

        int currentAmmo = inventory.GetCurrentAmmo();
        int needed = weaponData.magazineSize - currentAmmo;

        if (needed <= 0) return;

        int available = inventory.GetReserveAmmo(weaponData.ammoType);
        int toLoad = Mathf.Min(needed, available);

        if (toLoad <= 0)
        {
            Debug.Log("No reserve ammo left!");
            return;
        }

        // Take from pool, put into gun
        inventory.ConsumeReserveAmmo(weaponData.ammoType, toLoad);
        inventory.SetCurrentAmmo(currentAmmo + toLoad);

        Debug.Log($"Reloaded {toLoad} rounds. Mag now: {currentAmmo + toLoad}");
    }

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, 0.05f);
        }
    }
}