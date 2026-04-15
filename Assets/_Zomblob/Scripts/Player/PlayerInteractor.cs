using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float range = 2f;
    [SerializeField] private TMPro.TextMeshProUGUI promptText;
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
            CratePickup weaponCrate = hit.GetComponent<CratePickup>();
            AmmoPickup ammoCrate = hit.GetComponent<AmmoPickup>();

            if (weaponCrate == null && ammoCrate == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = (weaponCrate != null) ? (MonoBehaviour)weaponCrate : (MonoBehaviour)ammoCrate;
            }
        }

        if (closest != null)
        {
            // Turn the text ON
            promptText.gameObject.SetActive(true);

            // Figure out what to say
            if (closest is CratePickup weapon)
                promptText.text = $"[E] {weapon.GetInteractName()}";
            else if (closest is AmmoPickup ammo)
                promptText.text = "[E] Pick up Ammo";
        }
        else
        {
            if (promptText != null) promptText.gameObject.SetActive(false);
        }

        if (closest != null && Input.GetKeyDown(KeyCode.E))
        {
            if (closest is CratePickup weapon) weapon.Interact(inventory);
            else if (closest is AmmoPickup ammo) ammo.Interact(inventory);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}