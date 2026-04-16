using UnityEngine;
using System;
using UnityEngine.AI;

public class ZombieBossHealth : MonoBehaviour, IDamageable
{

    public static event Action EnemyDied;
    [Header("Health")]
    [SerializeField] private float maxBossHealth = 100f;
    private float currentBossHealth;

    private Rigidbody bRB;
    private NavMeshAgent bossAgent;
    private BossPool owningPool2;

    private void Awake()
    {
        bRB = GetComponent<Rigidbody>();
        bossAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable() //  Reset for pooling
    {
        currentBossHealth = maxBossHealth;

        if (bossAgent != null)
            bossAgent.enabled = true;

        if (bRB != null)
        {
            bRB.linearVelocity = Vector3.zero;
            bRB.angularVelocity = Vector3.zero;
            bRB.isKinematic = false;
        }
    }

    public void TakeDamage(float amount)
    {
        currentBossHealth -= amount;

        if (currentBossHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemyDied?.Invoke();

        if (bossAgent != null)
            bossAgent.enabled = false;

        if (bRB != null)
            bRB.isKinematic = true;

        gameObject.SetActive(false);
    }

    public void Init(BossPool bossPool)
    {
        owningPool2 = bossPool;
        currentBossHealth = maxBossHealth;

        if (bRB != null)
        {
            bRB.linearVelocity = Vector3.zero;
            bRB.angularVelocity = Vector3.zero;
            bRB.isKinematic = true;
        }

        if (bossAgent != null)
        {
            bossAgent.enabled = false;
            bossAgent.enabled = true;
        }
    }
}