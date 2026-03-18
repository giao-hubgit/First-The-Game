using UnityEngine;
using Unity.AI;
using NavMeshPlus.Components;

public class NavmeshManager : MonoBehaviour
{
    public NavMeshSurface navSurface;

    public float timeToWait = 1f;

    void Start()
    {
        Invoke("BakeMyMap", timeToWait);
    }

    void BakeMyMap()
    {
        navSurface.BuildNavMesh();
    }
}
