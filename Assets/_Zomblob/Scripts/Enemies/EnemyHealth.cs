using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Respawn (TEMP)")]
    [SerializeField] private bool enableRespawn = true;
    [SerializeField] private float respawnDelay = 3f;

    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashTime = 0.1f;

    [SerializeField] private float knockbackForce = 3f;
    private Rigidbody rb;

    private Color originalColor;
    private Vector3 spawnPosition;

    private Coroutine hitFlashRoutine;

    void Awake()
    {
        spawnPosition = transform.position;
        currentHealth = maxHealth;

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }

        if (enemyRenderer != null)
        {
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash());
            StartCoroutine(HitFlash());
        }

        if (rb != null)
        {
            Vector3 knockDir = -transform.forward;
            rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
        }
    }

    void Die()
    {
        if (enableRespawn)
        {
            StartCoroutine(Respawn());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Respawn()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        foreach (var c in GetComponents<Collider>())
            c.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        transform.position = spawnPosition;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        foreach (var c in GetComponents<Collider>())
            c.enabled = true;
    }

    IEnumerator HitFlash()
    {
        enemyRenderer.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        enemyRenderer.material.color = originalColor;
    }
}
