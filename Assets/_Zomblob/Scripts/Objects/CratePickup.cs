using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CratePickup : MonoBehaviour
{
    public enum CrateType { Fixed, Random }
    public CrateType crateType = CrateType.Fixed;

    [Header("Fixed Pickup")]
    public GameObject weaponPrefab;

    [Header("Random Loot Table")]
    public List<LootItem> lootTable = new List<LootItem>();

    [SerializeField] private TMPro.TextMeshProUGUI lootText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spinSound;


    [System.Serializable]
    public class LootItem
    {
        public string itemName;
        public GameObject weapon;
        public int weight;
        public bool isNothing;
    }

    private bool isSpinning = false;
    private float wiggleTimer = 0f;
    private List<LootItem> spinSequence = new List<LootItem>();

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isSpinning) return;

        wiggleTimer += Time.deltaTime * 3f;
        float angle = Mathf.Sin(wiggleTimer) * 5f;

        transform.rotation = initialRotation * Quaternion.Euler(0, angle, 0);
    }

    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null || isSpinning) return;

        if (crateType == CrateType.Fixed)
        {
            if (weaponPrefab != null)
            {
                inventory.Pickup(weaponPrefab);
                Destroy(gameObject);
            }
        }
        else
        {
            StartCoroutine(SpinAndGive(inventory));
        }
    }

    private IEnumerator SpinAndGive(PlayerInventory inventory)
    {
        isSpinning = true;

        if (audioSource == null || spinSound == null)
        {
            Debug.LogError("Audio missing!");
        }
        else
        {
            audioSource.PlayOneShot(spinSound);
        }

        float duration = spinSound.length; // ~2.424s
        float timer = 0f;
        LootItem currentItem = null;

        // SCROLLING LOOP
        while (timer < duration)
        {
            float progress = timer / duration;
            float delay = Mathf.Lerp(0.05f, 0.25f, progress);

            currentItem = lootTable[Random.Range(0, lootTable.Count)];
            ShowUI(currentItem);

            timer += delay;
            yield return new WaitForSeconds(delay);
        }

        // FINAL RESULT
        LootItem selectedItem = GetWeightedRandomItem();
        ShowUI(selectedItem);

        yield return new WaitForSeconds(1.0f);

        if (selectedItem != null && !selectedItem.isNothing && selectedItem.weapon != null)
        {
            inventory.Pickup(selectedItem.weapon);
        }
        else
        {
            Debug.Log("Crate was empty!");
        }

        HideUI();
        transform.rotation = initialRotation;
        isSpinning = false;

        Destroy(gameObject);
    }

    private LootItem GetWeightedRandomItem()
    {
        int totalWeight = 0;
        foreach (var item in lootTable) totalWeight += item.weight;

        int random = Random.Range(0, totalWeight);

        foreach (var item in lootTable)
        {
            if (random < item.weight)
                return item;

            random -= item.weight;
        }
        return null;
    }

    private void ShowUI(LootItem item)
    {
        if (lootText == null) return;
        lootText.gameObject.SetActive(true);

        if (item.isNothing || item.weapon == null)
        {
            lootText.text = "[ NOTHING ]";
            lootText.color = Color.gray;
        }
        else
        {
            string displayName = !string.IsNullOrEmpty(item.itemName) ? item.itemName : item.weapon.name;
            lootText.text = "[" + displayName + "]";

            // RARITY COLORS
            if (item.weight >= 25) lootText.color = Color.white;      // common
            else if (item.weight >= 10) lootText.color = Color.cyan;   // rare
            else if (item.weight >= 3) lootText.color = new Color(1f, 0.5f, 0f); // orange/epic
            else lootText.color = Color.magenta; // ultra rare/legendary
        }
    }

    private void HideUI()
    {
        if (lootText != null)
        {
            lootText.text = "";
            lootText.gameObject.SetActive(false);
        }
    }

    public string GetInteractName()
    {
        if (isSpinning) return "Rolling...";

        if (crateType == CrateType.Random)
            return "Open Mystery Crate";

        return (weaponPrefab != null) ? "Pick up " + weaponPrefab.name : "Pick up Weapon";
    }
}