using UnityEngine;
using System.Collections.Generic;

public class ZombiePool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private readonly Queue<GameObject> pool = new();

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);

        // Reset health + link pool
        var health = obj.GetComponent<EnemyHealth>();
        health.Init(this);

        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}