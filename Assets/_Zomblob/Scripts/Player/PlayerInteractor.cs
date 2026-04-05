using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float range = 2f;
    private PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        MonoBehaviour closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // Look for both types of pickups
            CratePickup weaponCrate = hit.GetComponent<CratePickup>();
            AmmoPickup ammoCrate = hit.GetComponent<AmmoPickup>();

            if (weaponCrate == null && ammoCrate == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                // Store whichever one we found
                closest = (weaponCrate != null) ? (MonoBehaviour)weaponCrate : (MonoBehaviour)ammoCrate;
            }
        }

        // Interaction Logic
        if (closest != null && Input.GetKeyDown(KeyCode.E))
        {
            if (closest is CratePickup weapon)
            {
                weapon.Interact(inventory);
            }
            else if (closest is AmmoPickup ammo)
            {
                ammo.Interact(inventory);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}