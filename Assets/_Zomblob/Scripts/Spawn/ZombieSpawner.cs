using UnityEngine;
using System;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public event Action EnemySpawned;

    [SerializeField] private ZombiePool pool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenSpawns = 0.5f;

    public void BeginWave(int count)
    {
        StopAllCoroutines();
        StartCoroutine(SpawnRoutine(count));
    }

    IEnumerator SpawnRoutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            pool.Get(sp.position, sp.rotation);
            EnemySpawned?.Invoke();

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}