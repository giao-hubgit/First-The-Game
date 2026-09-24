using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (objectToSpawn == null)
        {
            Pool poolInfo = pools.Find(p => p.tag == tag);
            objectToSpawn = Instantiate(poolInfo.prefab);
            objectToSpawn.transform.SetParent(this.transform);
        }

        if (objectToSpawn.activeSelf)
        {
            Pool poolInfo = pools.Find(p => p.tag == tag);

            GameObject newObj = Instantiate(poolInfo.prefab);
            newObj.transform.SetParent(this.transform);

            poolDictionary[tag].Enqueue(objectToSpawn);

            objectToSpawn = newObj;
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        TrailRenderer[] trails = objectToSpawn.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
        }

        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}