using UnityEngine;

public class ParticleAlwaysUp : MonoBehaviour
{
    public Vector3 fixedRotation = new Vector3(-90f, 0f, 0f);

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
