using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public event Action EnemySpawned;

    [SerializeField] private ZombiePool pool;
    [SerializeField] private BossPool bossPool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenSpawns = 0.5f;

    [Header("NavMesh Settings")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Transform player;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    private Coroutine currentWaveRoutine;

    
    public IEnumerator BeginWaveRoutine(int count, int waveIndex)
    {
        if (currentWaveRoutine != null)
            StopCoroutine(currentWaveRoutine);

        float spawnDelay = Mathf.Max(0.15f, timeBetweenSpawns - waveIndex * 0.05f);

        Debug.Log($"Spawning {count} zombies | Delay: {spawnDelay}");

        yield return StartCoroutine(SpawnRoutine(count, spawnDelay));

        Debug.Log("Wave spawning finished.");
    }

    
    // public void BeginWave(int count, int waveIndex)
    // {
    //     StartCoroutine(BeginWaveRoutine(count, waveIndex));
    // }

    private IEnumerator SpawnRoutine(int count, float spawnDelay)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();

            Debug.Log($"Spawned enemy {i + 1}/{count}");

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        var sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = GetSafeNavMeshPosition(sp.position, spawnRadius);

        GameObject enemy = pool.Get(spawnPos, Quaternion.identity);

        enemy.GetComponent<EnemyHealth>()?.Init(pool);

        EnemySpawned?.Invoke();
    }

    private Vector3 GetSafeNavMeshPosition(Vector3 center, float radius)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * radius;
            randomPoint.y = center.y;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                if (player == null ||
                    Vector3.Distance(hit.position, player.position) >= minDistanceFromPlayer)
                {
                    return hit.position;
                }
            }
        }

        return center;
    }

    public void SpawnBoss()
    {
        var sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = GetSafeNavMeshPosition(sp.position, spawnRadius);

        GameObject boss = bossPool.GetBoss(spawnPos, Quaternion.identity);

        boss.GetComponent<ZombieBossHealth>()?.Init(bossPool);

        EnemySpawned?.Invoke();

        Debug.Log("Boss Spawned!");
    }
}