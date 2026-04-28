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

    public GameObject GetBoss(Vector3 position, Quaternion rotation)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        // Pool exhausted, instantiate new enemy
        GameObject newObj = Instantiate(Boss_PreFab, position, rotation);
        pool.Add(newObj);
        return newObj;
    }
}
