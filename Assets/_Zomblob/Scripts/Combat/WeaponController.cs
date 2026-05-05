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

    [Header("Magazine")]
    [SerializeField] private GameObject magVisual;

    [Header("Magazine Drop")]
    [SerializeField] private Transform magSocket;
    [SerializeField] private GameObject magDropPrefab;

    [Header("Magazine Drop Scaling")]
    [SerializeField] private Vector3 magDropScale = Vector3.one;

    private bool hasDroppedMag = false;

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

    [Header("Muzzle Flash")]
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Header("Casing Ejection")]
    [SerializeField] private Transform casingEjectPoint;
    [SerializeField] private GameObject casingPrefab;

    private bool isReloading = false;
    private Coroutine reloadCoroutine;

    private float nextFireTime;
    private float lastFireTime;
    private bool isBursting = false;

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (fireInput == null || firePoint == null || inventory == null) return;

        if (isReloading) return;

        HandleDynamicSpread();

        if (!Input.GetMouseButton(0))
        {
            currentSpread = Mathf.Lerp(currentSpread, weaponData.spread, Time.deltaTime * (spreadRecoverySpeed * 1.5f));
        }

        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.forward;
        dir.y = 0;
        dir.Normalize();

        bool canFire = Time.time >= nextFireTime && !isReloading;

        if (weaponData.fireMode == FireMode.FullAuto)
        {
            // Full auto check
            if (Input.GetMouseButton(0) && canFire)
            {
                Fire(origin, dir);
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
        else if (weaponData.fireMode == FireMode.SemiAuto)
        {
            // Semi auto check
            if (Input.GetMouseButtonDown(0) && canFire)
            {
                Fire(origin, dir);
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
        else if (weaponData.fireMode == FireMode.Burst)
        {
            // Burst check
            if (Input.GetMouseButtonDown(0) && canFire && !isBursting)
            {
                StartCoroutine(BurstFire());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            reloadCoroutine = StartCoroutine(ReloadRoutine());
        }

        /*
         * if (Input.GetKeyDown(KeyCode.T))
        {
            if (magVisual != null)
            {
                magVisual.SetActive(!magVisual.activeSelf);
                Debug.Log($"Mag visibility toggled: {magVisual.activeSelf}");
            }
            else
            {
                Debug.LogError("MagVisual is NULL! Check your Inspector assignment.");
            }
        }
        */
    }

    private void HandleDynamicSpread()
    {
        if (weaponData == null) return;

        float baseSpread = weaponData.spread;

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

        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadRecoverySpeed);
    }

    private Vector3 GetSpreadDirection(Vector3 baseDir)
    {
        float spread = currentSpread;
        Quaternion spreadRotation = Quaternion.Euler(0, Random.Range(-spread * 50f, spread * 50f), 0);
        return (spreadRotation * baseDir).normalized;
    }

    IEnumerator BurstFire()
    {
        isBursting = true;
        int shots = 3;
        for (int i = 0; i < shots; i++)
        {
            if (isReloading)
                break;

            if (inventory.GetCurrentAmmo() <= 0)
                break;

            Fire(firePoint.position, firePoint.forward);

            yield return new WaitForSeconds(1f / fireRate);
        }
        nextFireTime = Time.time + (1f / fireRate);
        isBursting = false;
    }

    void Fire(Vector3 origin, Vector3 dir)
    {
        if (isReloading) return;

        int currentAmmo = inventory.GetCurrentAmmo();
        if (currentAmmo <= 0) return;

        SpawnMuzzleFlash();
        PlayFireSound();

        if (weaponData.weaponType != WeaponType.Shotgun)
        {
            EjectCasing();
        }

        currentAmmo--;
        inventory.SetCurrentAmmo(currentAmmo);

        currentSpread += spreadIncreasePerShot;

        float maxSpread = weaponData.spread * maxSpreadMultiplier;
        currentSpread = Mathf.Min(currentSpread, maxSpread);

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
                float shotgunSpread = currentSpread + weaponData.spread;

                Quaternion spreadRotation = Quaternion.Euler(0, Random.Range(-shotgunSpread * 50f, shotgunSpread * 50f), 0);
                Vector3 pelletDir = spreadRotation * dir;

                ProcessShot(origin, pelletDir);
            }
        }
        else
        {
            ProcessShot(origin, GetSpreadDirection(dir));
        }
    }

    void PlayFireSound()
    {
        if (weaponData == null || weaponData.fireSound == null) return;

        float pitchVariance = weaponData.fireRate > 8 ? 0.04f : 0.02f;
        audioSource.pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
        audioSource.PlayOneShot(weaponData.fireSound, weaponData.fireVolume);
    }

    void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || firePoint == null) return;

        GameObject flash = Instantiate(
            muzzleFlashPrefab,
            firePoint.position,
            firePoint.rotation
        );

        flash.transform.SetParent(firePoint);

        var ps = flash.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;

            float baseSize = 0.2f;
            main.startSize = baseSize * weaponData.muzzleFlashScale;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
            new ParticleSystem.Burst(0, (short)weaponData.muzzleFlashCount)
            });
        }

        flash.transform.Rotate(0, 0, Random.Range(0, 360));
        Destroy(flash, 0.08f);
    }

    void EjectCasing()
    {
        if (casingPrefab == null || casingEjectPoint == null) return;

        GameObject casing = Instantiate(
            casingPrefab,
            casingEjectPoint.position,
            casingEjectPoint.rotation
        );

        casing.transform.SetParent(null);

        Rigidbody rb = casing.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // BASE EJECTION DIRECTION (RIGHT SIDE + UP)
            Vector3 ejectDir =
                casingEjectPoint.right * Random.Range(1.5f, 2.5f) +
                casingEjectPoint.up * Random.Range(0.8f, 1.5f);

            float forceMultiplier = 1f;

            switch (weaponData.weaponType)
            {
                case WeaponType.Glock:
                case WeaponType.MP5:
                    forceMultiplier = 1.2f;
                    break;

                case WeaponType.AK47:
                    forceMultiplier = 1.5f;
                    break;

                case WeaponType.M16A1:
                    forceMultiplier = 1.3f;
                    break;

                case WeaponType.Barrett:
                    forceMultiplier = 0.8f;
                    break;

                case WeaponType.Shotgun:
                    forceMultiplier = 1.0f;
                    break;
            }

            rb.AddForce(ejectDir * forceMultiplier, ForceMode.Impulse);

            rb.AddTorque(
                new Vector3(
                    Random.Range(2f, 6f),
                    Random.Range(2f, 6f),
                    Random.Range(2f, 6f)
                ),
                ForceMode.Impulse
            );
        }

        Destroy(casing, 10f);
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

    IEnumerator ReloadRoutine()
    {
        if (weaponData == null || inventory == null) yield break;

        // Check if we even need to reload
        int currentAmmo = inventory.GetCurrentAmmo();
        if (currentAmmo >= weaponData.magazineSize) yield break;

        int available = inventory.GetReserveAmmo(weaponData.ammoType);
        if (available <= 0) yield break;

        isReloading = true;

        // Route to the correct reload logic
        if (weaponData.reloadType == ReloadType.PerShell)
        {
            yield return StartCoroutine(ReloadPerShell());
        }
        else
        {
            yield return StartCoroutine(ReloadMagazine());
        }

        isReloading = false;
    }

    IEnumerator ReloadPerShell()
    {
        while (true)
        {
            int currentAmmo = inventory.GetCurrentAmmo();
            int reserve = inventory.GetReserveAmmo(weaponData.ammoType);

            if (currentAmmo >= weaponData.magazineSize) break;
            if (reserve <= 0) break;

            if (Input.GetMouseButtonDown(0)) break;

            PlayReloadSound();

            yield return new WaitForSeconds(weaponData.reloadTime);

            inventory.SetCurrentAmmo(currentAmmo + 1);
            inventory.ConsumeReserveAmmo(weaponData.ammoType, 1);
        }
    }

    IEnumerator ReloadMagazine()
    {
        int currentAmmo = inventory.GetCurrentAmmo();
        int needed = weaponData.magazineSize - currentAmmo;
        int available = inventory.GetReserveAmmo(weaponData.ammoType);

        PlayReloadSound();

        // EMPTY MAG DROP
        if (currentAmmo == 0)
        {
            HandleEmptyMag();
        }

        yield return new WaitForSeconds(weaponData.reloadTime);

        int toLoad = Mathf.Min(needed, available);
        inventory.ConsumeReserveAmmo(weaponData.ammoType, toLoad);
        inventory.SetCurrentAmmo(currentAmmo + toLoad);

        hasDroppedMag = false;
        if (magVisual != null) magVisual.SetActive(true);
    }

    void PlayReloadSound()
    {
        if (weaponData == null || weaponData.reloadSound == null) return;

        if (weaponData.reloadType == ReloadType.Magazine)
        {
            float clipLength = weaponData.reloadSound.length;
            float targetTime = weaponData.reloadTime;

            float pitch = clipLength / targetTime;
            pitch = Mathf.Clamp(pitch, 0.7f, 1.3f);

            audioSource.pitch = pitch;
        }
        else
        {
            audioSource.pitch = 1f;
        }

        audioSource.PlayOneShot(weaponData.reloadSound, weaponData.reloadVolume);
    }

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, 0.05f);
        }
    }

    void HandleEmptyMag()
    {
        if (hasDroppedMag) return;
        hasDroppedMag = true;

        if (magVisual != null)
            magVisual.SetActive(false);

        if (magDropPrefab != null && magSocket != null)
        {
            GameObject drop = Instantiate(magDropPrefab, magSocket.position, magSocket.rotation);

            drop.transform.SetParent(null, true);

            Vector3 baseScale = drop.transform.localScale;
            drop.transform.localScale = Vector3.Scale(baseScale, magDropScale);

            Debug.Log("Base Scale: " + baseScale);
            Debug.Log("Final Scale: " + drop.transform.localScale);

            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Vector3 force = transform.forward * 1.5f + Vector3.up * 1.5f;
                rb.AddForce(force, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
            }

            Destroy(drop, 1.5f);
        }

        StartCoroutine(RestoreMagAfterDelay());
    }

    IEnumerator RestoreMagAfterDelay()
    {
        float delay = weaponData != null ? weaponData.reloadTime : 1f;

        yield return new WaitForSeconds(delay);

        if (magVisual != null)
            magVisual.SetActive(true);

        hasDroppedMag = false;
    }
    IEnumerator ApplyScaleNextFrame(GameObject obj)
    {
        yield return null; // Wait 1 frame for Unity to finish its internal setup
        if (obj != null)
        {
            obj.transform.localScale = magDropScale;
        }
    }

    IEnumerator DebugScale(GameObject obj)
    {
        // We wait 0.1s to make sure Unity has finished all internal "Update" loops
        yield return new WaitForSeconds(0.1f);

        if (obj != null)
        {
            Debug.Log("Scale AFTER 0.1s: " + obj.transform.localScale);
        }
    }
}