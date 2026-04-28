using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private int baseCount = 5;
    [SerializeField] private float breakBetweenWaves = 5f;
    [SerializeField] private int bossEveryNWaves = 5;
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private int waveCount = 0;

    private int waveIndex = 0;
    private int alive = 0;
    private bool wavesActive = true;

    private void OnEnable()
    {
        spawner.EnemySpawned += OnEnemySpawned;
        EnemyHealth.EnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        spawner.EnemySpawned -= OnEnemySpawned;
        EnemyHealth.EnemyDied -= OnEnemyDied;
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
{
    while (wavesActive && waveIndex < maxWaves)
    {
        waveIndex++;
        waveCount = waveIndex;

        bool isBossWave = waveIndex % bossEveryNWaves == 0;

        int toSpawn = baseCount + waveIndex * 2;

        if (isBossWave)
            toSpawn = Mathf.RoundToInt(toSpawn * 0.7f);

        alive = 0;

        Debug.Log($"========== WAVE {waveIndex}/{maxWaves} ==========");

        // IMPORTANT CHANGE:
        // Wait until ALL zombies for this wave finish spawning
        yield return StartCoroutine(
            spawner.BeginWaveRoutine(toSpawn, waveIndex)
        );

        // Spawn boss after normal zombies finish spawning
        if (isBossWave)
        {
            spawner.SpawnBoss();
        }

        // IMPORTANT CHANGE:
        // Wait until every spawned enemy is dead
        while (alive > 0)
        {
            Debug.Log($"Waiting... Enemies Alive: {alive}");
            yield return null;
        }

        Debug.Log($"Wave {waveIndex} cleared!");

        // If final wave done -> stop loop
        if (waveIndex >= maxWaves)
            break;

        yield return new WaitForSeconds(breakBetweenWaves);
    }

}

    private void OnEnemySpawned() => alive++;
    private void OnEnemyDied() => alive = Mathf.Max(0, alive - 1);

    public void StopWaves() => wavesActive = false;
}