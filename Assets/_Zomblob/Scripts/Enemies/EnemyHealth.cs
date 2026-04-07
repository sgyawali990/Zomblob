using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event Action EnemyDied;

    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Visual")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashTime = 0.1f;
    private Color originalColor;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;
    private bool isKnockback = false;

    [Header("DeathTimer")]
    [SerializeField] private float deathTimer = 2f;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private ZombiePool owningPool;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        mainCamera = Camera.main;

        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HitFlash());

        if (!isKnockback)
            StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        if (agent != null) agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 knockDir = transform.position - mainCamera.transform.position;
            knockDir.y = 0;
            knockDir.Normalize();

            rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.4f);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (agent != null && currentHealth > 0) agent.enabled = true;
        isKnockback = false;
    }

    private void Die()
    {
        EnemyDied?.Invoke();

        if (agent != null) agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }

        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(deathTimer);

        if (owningPool != null)
            owningPool.Release(gameObject);
        else
            Destroy(gameObject);
    }

    private IEnumerator HitFlash()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(flashTime);
            enemyRenderer.material.color = originalColor;
        }
    }

    public void Init(ZombiePool pool)
    {
        owningPool = pool;
        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (agent != null)
        {
            agent.enabled = false;
            agent.enabled = true;
        }
    }
}