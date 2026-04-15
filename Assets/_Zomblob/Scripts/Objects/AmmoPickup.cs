using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30;
    public int pityAmmo = 15;

    void Update()
    {
        transform.Rotate(0, 60f * Time.deltaTime, 0);
    }
    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null) return;

        bool gaveAmmo = false;

        // SLOT 1
        if (inventory.slot1 != null)
        {
            WeaponData d1 = inventory.slot1.GetComponent<WeaponData>();

            if (d1 != null && d1.isGun)
            {
                inventory.AddReserveAmmo(d1.ammoType, ammoAmount);
                gaveAmmo = true;
            }
        }

        // SLOT 2
        if (inventory.slot2 != null)
        {
            WeaponData d2 = inventory.slot2.GetComponent<WeaponData>();

            if (d2 != null && d2.isGun)
            {
                inventory.AddReserveAmmo(d2.ammoType, ammoAmount);
                gaveAmmo = true;
            }
        }

        // PITY SYSTEM
        if (!gaveAmmo)
        {
            inventory.AddReserveAmmo(AmmoType.NineMM, pityAmmo);
        }

        Debug.Log("Ammo picked up");

        Destroy(gameObject);
    }

    public string GetInteractName()
    {
        // Simple and clean for ammo
        return "Pick up Ammo";
    }

}