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
        rb = GetComponent<Rigidbody>();

        spawnPosition = transform.position;
        currentHealth = maxHealth;

        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (enemyRenderer != null)
        {
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash());
        }

        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 knockDir = transform.position - Camera.main.transform.position;
            knockDir.y = 0;
            knockDir.Normalize();

            rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
        }

        StartCoroutine(ReenableAgent(agent));
    }

    // Turn AI back on after knockback
    IEnumerator ReenableAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        yield return new WaitForSeconds(0.4f);

        if (rb != null)
        {
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        if (agent != null && currentHealth > 0)
            agent.enabled = true;
    }

    void Die()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 6f, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(2f);

        if (enableRespawn)
            StartCoroutine(Respawn());
        else
            Destroy(gameObject);
    }

    IEnumerator Respawn()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        foreach (var c in GetComponents<Collider>())
            c.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        foreach (var c in GetComponents<Collider>())
            c.enabled = true;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.enabled = true;
    }

    IEnumerator HitFlash()
    {
        enemyRenderer.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        enemyRenderer.material.color = originalColor;
    }
}