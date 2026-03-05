using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;


public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    [SerializeField] public Transform playerTransform;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCoolDown = 1f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = rotationSpeed;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Time.time < lastAttackTime)
            return;

        if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth player))
        {
            player.TakeDamage((int)damage);

            Debug.Log("Enemy collided with player!");

            lastAttackTime = Time.time + attackCoolDown;
        }
    }
}
