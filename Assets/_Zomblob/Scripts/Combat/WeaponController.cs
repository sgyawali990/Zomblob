using UnityEngine;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    public FireInput fireInput;
    public PlayerController playerController;

    [SerializeField] private LineRenderer line;

    public Transform firePoint;
    public float fireRate = 5f;
    public float range = 100f;
    public float damage = 10f; // Temporary, change as needed

    private float nextFireTime;

    void Start()
    {
        if (line != null)
            line.positionCount = 2;
    }

    void Update()
    {
        if (fireInput == null || playerController == null || firePoint == null)
            return;

        Vector3 origin = firePoint.position;

        // Compute direction from muzzle ? aim point
        Vector3 dir = (playerController.AimPoint - origin).normalized;

        // Update aim line every frame
        if (dir.sqrMagnitude > 0.001f && line != null)
        {
            line.SetPosition(0, origin);
            line.SetPosition(1, origin + dir * range);
        }

        // Fire logic
        if (fireInput.IsFiring && Time.time >= nextFireTime)
        {
            Fire(origin, dir);
            nextFireTime = Time.time + (1f / fireRate);
        }

    }

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, 0.05f);
        }
    }

    void Fire(Vector3 origin, Vector3 dir)
    {
        // Safety check
        if (dir.sqrMagnitude < 0.001f)
            return;

        Ray ray = new Ray(origin, dir);

        // Debug ray
        Debug.DrawRay(origin, dir * range, Color.red, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}