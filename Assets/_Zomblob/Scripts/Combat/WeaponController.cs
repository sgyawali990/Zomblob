using UnityEngine;
using System.Collections;

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
        if (fireInput == null) fireInput = Object.FindFirstObjectByType<FireInput>();
        if (playerController == null) playerController = Object.FindFirstObjectByType<PlayerController>();

        inventory = Object.FindFirstObjectByType<PlayerInventory>();
        weaponData = GetComponent<WeaponData>();

        if (weaponData != null)
        {
            fireRate = weaponData.fireRate;
            range = weaponData.range;
            damage = weaponData.damage;
        }

        if (line != null) line.positionCount = 2;
    }

    void Update()
    {
        if (fireInput == null || firePoint == null || inventory == null) return;

        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.forward;

        if (dir.sqrMagnitude > 0.001f && line != null)
        {
            line.SetPosition(0, origin);
            line.SetPosition(1, origin + dir * range);
        }

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
        else if (weaponData.fireMode == FireMode.Burst)
        {
            if (Input.GetMouseButtonDown(0) && canFire && !isBursting)
            {
                StartCoroutine(BurstFire());
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) Reload();
    }

    IEnumerator BurstFire()
    {
        isBursting = true;
        int shots = 3;
        for (int i = 0; i < shots; i++)
        {
            if (inventory.GetCurrentAmmo() <= 0) break;
            Fire(firePoint.position, firePoint.forward);
            yield return new WaitForSeconds(1f / fireRate);
        }
        nextFireTime = Time.time + (1f / fireRate);
        isBursting = false;
    }

    void Fire(Vector3 origin, Vector3 dir)
    {
        int currentAmmo = inventory.GetCurrentAmmo();
        if (currentAmmo <= 0) return;

        currentAmmo--;
        inventory.SetCurrentAmmo(currentAmmo);

        // SHOTGUN LOGIC
        if (weaponData.weaponType == WeaponType.Shotgun)
        {
            int pellets = 8;
            for (int i = 0; i < pellets; i++)
            {
                // Calculate spread for each pellet
                Vector3 spreadDir = dir + new Vector3(
                    Random.Range(-weaponData.spread, weaponData.spread),
                    Random.Range(-weaponData.spread, weaponData.spread),
                    Random.Range(-weaponData.spread, weaponData.spread)
                );
                spreadDir.Normalize();

                Debug.DrawRay(origin, spreadDir * range, Color.yellow, 0.1f);

                PerformRaycast(origin, spreadDir);
            }
        }
        else // NORMAL WEAPON LOGIC
        {
            PerformRaycast(origin, dir);
        }
    }

    // Extracted Raycast logic to keep Fire() clean
    void PerformRaycast(Vector3 origin, Vector3 dir)
    {
        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }

            if (line != null && weaponData.weaponType != WeaponType.Shotgun)
            {
                line.SetPosition(0, origin);
                line.SetPosition(1, hit.point);
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
        if (toLoad <= 0) return;

        inventory.ConsumeReserveAmmo(weaponData.ammoType, toLoad);
        inventory.SetCurrentAmmo(currentAmmo + toLoad);
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