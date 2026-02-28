using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;          // Tên định danh (VD: "Bullet", "EnemyDeathVFX")
        public GameObject prefab;   // Prefab thực tế
        public int size;            // Số lượng khởi tạo sẵn
    }

    public static ObjectPooler Instance; // Singleton

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform); // Gom nhóm vào Pooler cho gọn Hierarchy
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool với tag " + tag + " không tồn tại!");
            return null;
        }

        // 1. Lấy thử object đầu tiên trong hàng đợi ra để kiểm tra
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // 2. KIỂM TRA: Nếu object này đang Active, nghĩa là toàn bộ Pool đều đang bận
        if (objectToSpawn.activeSelf)
        {
            // Tạo thêm 1 cái mới y hệt để mở rộng Pool
            // Tìm lại thông tin pool từ list 'pools' dựa vào tag
            Pool poolInfo = pools.Find(p => p.tag == tag);

            // Tạo thêm bản sao
            GameObject newObj = Instantiate(poolInfo.prefab);
            newObj.transform.SetParent(this.transform);

            // Đẩy object cũ (đang bận) ngược lại vào hàng đợi để không làm mất nó
            poolDictionary[tag].Enqueue(objectToSpawn);

            // Gán objectToSpawn thành cái mới vừa tạo
            objectToSpawn = newObj;
        }

        // 3. Thiết lập thông số và kích hoạt
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // 4. Đẩy lại vào cuối hàng đợi để tái sử dụng
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}