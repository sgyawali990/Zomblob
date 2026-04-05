using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText; 
    public PlayerInventory inventory;

    void Update()
    {
        if (inventory == null || ammoText == null) return;

        GameObject currentWeapon = inventory.GetCurrentWeaponInstance();

        // If hands are empty, show nothing
        if (currentWeapon == null)
        {
            ammoText.text = "";
            return;
        }

        WeaponData data = currentWeapon.GetComponent<WeaponData>();

        // Logic for melee
        if (data == null || !data.isGun)
        {
            ammoText.text = "MELEE";
            ammoText.color = Color.white;
            return;
        }

        // Logic for scarcity
        int currentMag = inventory.GetCurrentAmmo();
        int reserve = inventory.GetReserveAmmo(data.ammoType);

        if (currentMag <= 0 && reserve <= 0)
        {
            ammoText.text = "OUT OF AMMO";
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.text = $"{currentMag} / {reserve}";
            ammoText.color = Color.white;
        }
    }
}