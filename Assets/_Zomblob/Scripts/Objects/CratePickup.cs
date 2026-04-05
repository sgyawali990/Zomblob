using UnityEngine;

public class CratePickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject weaponPrefab;
    public string pickupPrompt = "Press E to pick up weapon";

    // Called by the PlayerPickupInteractor when E is pressed
    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null) return;

        if (weaponPrefab != null)
        {
            // Tell the inventory to handle the logic (Slot 1, Slot 2, or Replace)
            inventory.Pickup(weaponPrefab);

            // Destroy the crate once the item is taken
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("CratePickup: No weapon prefab assigned to this crate!");
        }
    }
}