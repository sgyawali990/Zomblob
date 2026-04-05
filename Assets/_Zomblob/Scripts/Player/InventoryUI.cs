using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;

    public Image slot1Image;
    public Image slot2Image;

    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;

    private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    private Color filledColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color equippedColor = new Color(0.8f, 0.8f, 0.2f, 0.9f);

    void Update()
    {
        if (inventory == null) return;

        UpdateSlot(inventory.slot1, slot1Text, slot1Image);
        UpdateSlot(inventory.slot2, slot2Text, slot2Image);

        HighlightEquipped();
    }

    void UpdateSlot(GameObject weapon, TextMeshProUGUI text, Image img)
    {
        if (weapon == null)
        {
            text.text = "";
            img.color = emptyColor;
            return;
        }

        WeaponData data = weapon.GetComponent<WeaponData>();

        if (data != null && !string.IsNullOrEmpty(data.displayName))
            text.text = data.displayName;
        else
            text.text = weapon.name;

        img.color = filledColor;
    }

    void HighlightEquipped()
    {
        int equipped = inventory.GetEquippedSlot();

        slot1Image.color = (inventory.slot1 == null) ? emptyColor : filledColor;
        slot2Image.color = (inventory.slot2 == null) ? emptyColor : filledColor;

        if (equipped == 0 && inventory.slot1 != null)
            slot1Image.color = equippedColor;
        else if (equipped == 1 && inventory.slot2 != null)
            slot2Image.color = equippedColor;
    }
}