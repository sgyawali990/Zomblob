using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    // How often we update the path (prevents spam)
    [SerializeField] private float repathRate = 0.25f;
    private float repathTimer;

    private NavMeshAgent agent;

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Movement
            agent.speed = moveSpeed;

            // NavMesh handles rotation
            agent.updateRotation = true;

            // These matter for turning
            agent.angularSpeed = 300f;   
            agent.acceleration = 20f;

            // Prevent zombie from walking into player
            agent.stoppingDistance = 1f;
        }

        // Auto-find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }


    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        if (playerTransform == null)
            return;

        // Repath timer
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            agent.SetDestination(playerTransform.position);
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