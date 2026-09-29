using UnityEngine;

[System.Serializable]
public class ObstacleItem
{
    public GameObject prefab;
    [Range(0f, 100f)]
    [Tooltip("Tỷ lệ % xuất hiện của Obstacle này")]
    public float spawnChance = 25f;
}

public class ObstacleTemplate : MonoBehaviour
{
    [Header("Obstacle List & Spawn Rates")]
    public ObstacleItem[] obstacles;

    public int minObstacle = 10;
    public int maxObstacle = 14;
    public int currentObstacle;
}