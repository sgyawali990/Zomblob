using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

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

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;

    private Rigidbody rb;
    private UnityEngine.AI.NavMeshAgent agent;
    private Color originalColor;

    private ZombiePool owningPool;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        currentHealth = maxHealth;

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

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 knockDir = transform.position - Camera.main.transform.position;
            knockDir.y = 0;
            knockDir.Normalize();

            rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
        }

        StartCoroutine(ReenableAgent());
    }

    

    IEnumerator ReenableAgent()
    {
        yield return new WaitForSeconds(0.4f);

        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
        }

        if (agent != null && currentHealth > 0)
            agent.enabled = true;
    }

    void Die()
    {
        EnemyDied?.Invoke();

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
        
        // null check
        if (owningPool != null)
        {
            owningPool.Release(gameObject);
        }
        else
        {
            // fallback for non-pooled zombies
            Destroy(gameObject);
        }
    }

    IEnumerator HitFlash()
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