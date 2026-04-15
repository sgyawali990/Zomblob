using UnityEngine;
using UnityEngine.UI;
using TMPro; // Added for text support

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float regenRate = 5f;
    [SerializeField] private float regenDelay = 3f;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;     
    [SerializeField] private Image damageFlashImage; 
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Settings")]
    [SerializeField] private float flashSpeed = 5f;
    private float currentHealth;
    private float lastDamageTime;
    private float flashAlpha = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        if (damageFlashImage != null) damageFlashImage.color = new Color(1, 0, 0, 0);
        UpdateUI();
    }

    void Update()
    {
        if (currentHealth < maxHealth && Time.time > lastDamageTime + regenDelay)
        {
            currentHealth += regenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            UpdateUI();
        }

        if (flashAlpha > 0)
        {
            flashAlpha -= flashSpeed * Time.deltaTime;
            flashAlpha = Mathf.Max(0, flashAlpha);
            damageFlashImage.color = new Color(1, 0, 0, flashAlpha);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        lastDamageTime = Time.time;
        flashAlpha = 0.4f;

        Debug.Log($"Took {damage} damage. HP: {currentHealth}");
        UpdateUI();

        if (currentHealth <= 0f) Die();
    }

    private void UpdateUI()
    {
        float ratio = currentHealth / maxHealth;

        if (healthSlider != null) healthSlider.value = ratio;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";

        if (fillImage != null)
        {
            if (ratio > 0.6f) fillImage.color = Color.green;
            else if (ratio > 0.3f) fillImage.color = Color.yellow;
            else fillImage.color = Color.red;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        gameObject.SetActive(false);
    }
}