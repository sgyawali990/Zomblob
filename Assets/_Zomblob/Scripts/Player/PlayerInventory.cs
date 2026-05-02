using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    public Transform weaponSocket;

    [Header("Slots")]
    public GameObject slot1;
    public GameObject slot2;

    [Header("Slot Colors")]
    public Color emptyColor = new Color(1, 1, 1, 0.2f);  
    public Color filledColor = Color.white;              
    public Color equippedColor = Color.green;

    [Header("Fallback")]
    public GameObject batPrefab;

    private int equippedSlot = -1;
    private GameObject currentWeaponInstance;

    [Header("Ammo Storage")]
    // Stored ammo
    public Dictionary<AmmoType, int> ammoReserve = new Dictionary<AmmoType, int>();

    // This is the ammo currently INSIDE the guns in each slot
    private int slot1Ammo;
    private int slot2Ammo;

    void Start()
    {
        if (weaponSocket == null)
            weaponSocket = transform.Find("PlayerModel/WeaponSocket");

        // Initialize starting reserve ammo
        ammoReserve[AmmoType.NineMM] = 0;
        ammoReserve[AmmoType.Rifle556] = 0;
        ammoReserve[AmmoType.Rifle762] = 0;
        ammoReserve[AmmoType.ShotgunShell] = 0;
        ammoReserve[AmmoType.FiftyCal] = 0;

        Equip(0);
    }

    // WeaponController will call these to see how much is in the mag
    public int GetCurrentAmmo()
    {
        return (equippedSlot == 0) ? slot1Ammo : slot2Ammo;
    }

    public void SetCurrentAmmo(int amount)
    {
        if (equippedSlot == 0) slot1Ammo = amount;
        else if (equippedSlot == 1) slot2Ammo = amount;
    }

    // This allows the UI to see what weapon we are actually holding
    public GameObject GetCurrentWeaponInstance()
    {
        return currentWeaponInstance;
    }

    // Standard backpack ammo logic
    public int GetReserveAmmo(AmmoType type) { return ammoReserve.ContainsKey(type) ? ammoReserve[type] : 0; }

    public void AddReserveAmmo(AmmoType type, int amount)
    {
        if (!ammoReserve.ContainsKey(type)) ammoReserve[type] = 0;
        ammoReserve[type] += amount;
    }

    public bool ConsumeReserveAmmo(AmmoType type, int amount)
    {
        if (GetReserveAmmo(type) < amount) return false;
        ammoReserve[type] -= amount;
        return true;
    }

    void Update()
    {
        // Toggle Slot 1: If already holding it, pull out the bat
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (equippedSlot == 0) EquipBat();
            else Equip(0);
        }

        // Toggle Slot 2: If already holding it, pull out the bat
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (equippedSlot == 1) EquipBat();
            else Equip(1);
        }

        // If you have no guns at all, make sure you're holding the bat
        if (slot1 == null && slot2 == null && currentWeaponInstance == null)
        {
            EquipBat();
        }
    }

    public void EquipBat()
    {
        equippedSlot = -1;

        if (currentWeaponInstance != null)
            Destroy(currentWeaponInstance);

        if (batPrefab != null)
        {
            currentWeaponInstance = Instantiate(batPrefab, weaponSocket);
            currentWeaponInstance.transform.localScale = Vector3.one;

            if (currentWeaponInstance.TryGetComponent<BatController>(out var bat))
            {
                bat.Initialize(this.transform);
            }

            AlignWeapon(currentWeaponInstance);
        }
        else
        {
            Debug.LogError("Bat Prefab NOT assigned in PlayerInventory!");
        }
    }

    // Initialize ammo on pickup
    public void Pickup(GameObject weaponPrefab)
    {
        WeaponData data = weaponPrefab.GetComponent<WeaponData>();

        if (slot1 == null)
        {
            slot1 = weaponPrefab;
            if (data != null) slot1Ammo = data.magazineSize; // Fill mag
            Equip(0);
        }
        else if (slot2 == null)
        {
            slot2 = weaponPrefab;
            if (data != null) slot2Ammo = data.magazineSize; // Fill mag
            Equip(1);
        }
        else
        {
            // Replace current slot
            if (equippedSlot == 0)
            {
                slot1 = weaponPrefab;
                if (data != null) slot1Ammo = data.magazineSize;
                Equip(0);
            }
            else
            {
                slot2 = weaponPrefab;
                if (data != null) slot2Ammo = data.magazineSize;
                Equip(1);
            }
        }
    }

    public int GetEquippedSlot()
    {
        return equippedSlot;
    }

    public void Equip(int slotIndex)
    {
        // If we are trying to equip an empty slot, just call our Bat helper and stop
        GameObject prefab = (slotIndex == 0) ? slot1 : slot2;
        if (prefab == null)
        {
            EquipBat();
            return;
        }

        // Otherwise, proceed with equipping the gun
        equippedSlot = slotIndex;

        if (currentWeaponInstance != null)
            Destroy(currentWeaponInstance);

        currentWeaponInstance = Instantiate(prefab, weaponSocket);
        currentWeaponInstance.transform.localScale = Vector3.one;

        AlignWeapon(currentWeaponInstance);
    }

    void AlignWeapon(GameObject weapon)
    {
        Transform grip = weapon.transform.Find("WeaponGrip");
        if (grip != null)
        {
            weapon.transform.position = weaponSocket.position - grip.localPosition;
            weapon.transform.rotation = weaponSocket.rotation * Quaternion.Inverse(grip.localRotation);
        }
        else
        {
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }
    }
}