using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    public GameObject[] Melee;
    public GameObject[] Ranged;

    public int maxEnemy;
    public int currentEnemy;

    void Awake()
    {
        maxEnemy = Random.Range(4, 6);
    }
}
