using UnityEngine;
using System.Collections.Generic;

public class ZombiePool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;
    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
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
        GameObject newObj = Instantiate(prefab, position, rotation);
        pool.Add(newObj);
        return newObj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
    }
}