using UnityEngine;
using Unity.AI;
using NavMeshPlus.Components;

public class NavmeshManager : MonoBehaviour
{
    public NavMeshSurface navSurface;

    public void BakeMyMap()
    {
        navSurface.BuildNavMesh();
    }
}
