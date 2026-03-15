using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public float delay = 0.5f;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), delay);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}