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
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private GameObject impactPrefab;

    [Header("Settings (Overridden by WeaponData if found)")]
    public float fireRate = 5f;
    public float range = 100f;
    public float damage = 10f;

    [Header("Dynamic Spread System")]
    [SerializeField] private float currentSpread;
    [SerializeField] private float maxSpreadMultiplier = 3f;
    [SerializeField] private float spreadIncreasePerShot = 0.02f;
    [SerializeField] private float spreadRecoverySpeed = 6f;
    [SerializeField] private float movementSpreadMultiplier = 1.5f;
    [SerializeField] private float firstShotAccuracyMultiplier = 0.5f;

    private float nextFireTime;
    private float lastFireTime;
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
            currentSpread = weaponData.spread;
        }

        if (line != null) line.positionCount = 2;
    }

    void Update()
    {
        if (fireInput == null || firePoint == null || inventory == null) return;

        HandleDynamicSpread();

        // Optional Feel Fix: Faster recovery when NOT firing
        if (!Input.GetMouseButton(0))
        {
            currentSpread = Mathf.Lerp(currentSpread, weaponData.spread, Time.deltaTime * (spreadRecoverySpeed * 1.5f));
        }

        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.forward;

        bool isAiming = Input.GetMouseButton(1);

        if (line != null)
        {
            line.enabled = isAiming;

            if (isAiming && dir.sqrMagnitude > 0.001f)
            {
                Ray ray = new Ray(origin, dir);
                Vector3 endPoint = origin + dir * range;

                if (Physics.Raycast(ray, out RaycastHit hit, range))
                {
                    endPoint = hit.point;
                }

                line.SetPosition(0, origin);
                line.SetPosition(1, endPoint);
            }
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

    private void HandleDynamicSpread()
    {
        if (weaponData == null) return;

        float baseSpread = weaponData.spread;

        // MOVEMENT PENALTY
        float movementFactor = 0f;
        if (playerController != null)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float speed = rb.linearVelocity.magnitude;
                movementFactor = Mathf.Clamp01(speed * 0.2f);
            }
        }

        float movementSpread = baseSpread * movementSpreadMultiplier * movementFactor;
        float targetSpread = baseSpread + movementSpread;

        // RECOVERY
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadRecoverySpeed);
    }

    private Vector3 GetSpreadDirection(Vector3 baseDir)
    {
        float spread = currentSpread;
        Vector3 offset = new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
        return (baseDir + offset).normalized;
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

        // BLOOM INCREASE
        currentSpread += spreadIncreasePerShot;

        // HARD CAP
        float maxSpread = weaponData.spread * maxSpreadMultiplier;
        currentSpread = Mathf.Min(currentSpread, maxSpread);

        // FIRST SHOT ACCURACY (tap fire reward)
        if (Time.time - lastFireTime > 0.3f)
        {
            currentSpread *= firstShotAccuracyMultiplier;
        }

        lastFireTime = Time.time;

        if (weaponData.weaponType == WeaponType.Shotgun)
        {
            int pellets = 8;
            for (int i = 0; i < pellets; i++)
            {
                // Shotgun Spread Fix: base + dynamic
                float shotgunSpread = currentSpread + weaponData.spread;
                Vector3 pelletDir = dir + new Vector3(
                    Random.Range(-shotgunSpread, shotgunSpread),
                    Random.Range(-shotgunSpread, shotgunSpread),
                    Random.Range(-shotgunSpread, shotgunSpread)
                );
                pelletDir.Normalize();

                ProcessShot(origin, pelletDir);
            }
        }
        else
        {
            ProcessShot(origin, GetSpreadDirection(dir));
        }
    }

    private void ProcessShot(Vector3 origin, Vector3 direction)
    {
        Vector3 endPoint = origin + direction * range;
        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            endPoint = hit.point;
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }

            if (impactPrefab != null)
            {
                Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        SpawnTracer(origin, endPoint);
    }

    void SpawnTracer(Vector3 start, Vector3 end)
    {
        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);

        TrailRenderer tr = tracer.GetComponent<TrailRenderer>();
        if (tr != null && weaponData != null)
        {
            float width = weaponData.tracerWidth * Random.Range(0.9f, 1.1f);
            tr.startWidth = width;
            tr.endWidth = 0f;
        }

        StartCoroutine(MoveTracer(tracer, start, end));
    }

    IEnumerator MoveTracer(GameObject tracer, Vector3 start, Vector3 end)
    {
        tracer.transform.position = start + (end - start).normalized * 0.01f;
        float time = 0;
        float duration = 0.08f;

        while (time < duration)
        {
            tracer.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        tracer.transform.position = end;
        Destroy(tracer, 0.05f);
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