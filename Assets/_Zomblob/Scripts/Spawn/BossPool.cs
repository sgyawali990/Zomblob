using UnityEngine;
using System.Collections.Generic;

public class BossPool : MonoBehaviour
{
    [SerializeField] private GameObject Boss_PreFab;
    [SerializeField] private int poolSize = 2;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject boss = Instantiate(Boss_PreFab);
            boss.SetActive(false);
            pool.Add(boss);
        }

    }

    public GameObject GetBoss()
    {
        foreach(var boss in pool)
        {
            if (!boss.activeInHierarchy)
            {
                return boss;
            }
        }
        Debug.LogWarning("No boss in pool");
        return null;
    }
}
