using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    [SerializeField] private float repathRate = 0.25f;

    private float repathTimer;

    private NavMeshAgent bossAgent;

    [Header ("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Damage")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime;

    void Awake()
    {
        bossAgent = GetComponent<NavMeshAgent>();

        if (bossAgent != null)
        {
            // Movement
            bossAgent.speed = moveSpeed;

            // NavMesh handles rotation
            bossAgent.updateRotation = true;

            // These matter for turning
            bossAgent.angularSpeed = 300f;   
            bossAgent.acceleration = 20f;

            // Prevent zombie from walking into player
            bossAgent.stoppingDistance = 1f;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    void Update()
    {
        if (bossAgent == null || !bossAgent.enabled || !bossAgent.isOnNavMesh)
            return;

        if (playerTransform == null)
            return;

        // Repath timer
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            bossAgent.SetDestination(playerTransform.position);
            repathTimer = repathRate;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Time.time < nextAttackTime)
            return;

        if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth player))
        {
            player.TakeDamage((int)damage);

            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
