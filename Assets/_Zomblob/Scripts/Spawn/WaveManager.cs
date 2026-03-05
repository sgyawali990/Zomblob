using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private ZombieSpawner spawner;

    [SerializeField] private int baseCount = 5;
    [SerializeField] private float breakBetweenWaves = 5f;

    private int waveIndex = 0;
    private int toSpawn;
    private int spawned;
    private int alive;

    private void OnEnable()
    {
        spawner.EnemySpawned += HandleEnemySpawned;
        EnemyHealth.EnemyDied += HandleEnemyDied; // static event example
    }

    private void OnDisable()
    {
        spawner.EnemySpawned -= HandleEnemySpawned;
        EnemyHealth.EnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (true)
        {
            waveIndex++;
            toSpawn = baseCount + waveIndex * 2;
            spawned = 0;

            spawner.BeginWave(toSpawn);

            // Wait until wave finished
            while (!(spawned >= toSpawn && alive == 0))
                yield return null;

            yield return new WaitForSeconds(breakBetweenWaves);
        }
    }

    private void HandleEnemySpawned()
    {
        spawned++;
        alive++;
    }

    private void HandleEnemyDied()
    {
        alive = Mathf.Max(0, alive - 1);
    }
}