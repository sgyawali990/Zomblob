using UnityEngine;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    public FireInput fireInput;
    public PlayerController playerController;

    public Transform firePoint;
    public float fireRate = 5f;
    public float range = 100f;

    private float nextFireTime;

    void Update()
    {
        if (fireInput == null || playerController == null) return;

        // Hold to fire with fire rate limit
        if (fireInput.IsFiring && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Fire()
    {
        Vector3 dir = playerController.AimDirection;

        // Safety check (no aim yet)
        if (dir.sqrMagnitude < 0.001f) return;

        Ray ray = new Ray(firePoint.position, dir);

        // Visual debug of shot direction
        Vector3 debugOrigin = playerController.transform.position + Vector3.up * 0.5f;
        Debug.DrawRay(debugOrigin, dir * range, Color.red, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            Debug.Log("Miss");
        }
    }
}