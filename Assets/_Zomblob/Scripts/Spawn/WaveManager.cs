using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private int baseCount = 5;
    [SerializeField] private float breakBetweenWaves = 5f;

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
        while (wavesActive)
        {
            waveIndex++;
            int toSpawn = baseCount + waveIndex * 2;
            alive = 0;

            spawner.BeginWave(toSpawn, waveIndex);

            while (alive > 0)
                yield return null;

            yield return new WaitForSeconds(breakBetweenWaves);
        }
    }

    private void OnEnemySpawned() => alive++;
    private void OnEnemyDied() => alive = Mathf.Max(0, alive - 1);

    public void StopWaves() => wavesActive = false;
}