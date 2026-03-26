using UnityEngine;

public class ObstacleTemplate : MonoBehaviour
{
    public GameObject[] Obstacles;

    public int maxObstacle;
    public int currentObstacle;

    void Awake()
    {
        maxObstacle = Random.Range(10, 14);
    }
}
