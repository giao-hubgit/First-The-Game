using UnityEngine;
using System.Collections;

public class EntityHurtsVFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material originalM;

    [SerializeField] private Material onDamageVFX_Mat;
    [SerializeField] private float onDamageVFX_Duration = 0.15f;
    private Coroutine OnDamageVFXCor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalM = sr.material;
    }

    public void PlayOnDamageVFX()
    {
        OnDamageVFXCor = StartCoroutine(OnDamageVFXCo());
    }

    private IEnumerator OnDamageVFXCo()
    {
        sr.material = onDamageVFX_Mat;

        yield return new WaitForSeconds(onDamageVFX_Duration);

        sr.material = originalM;
    }
}
