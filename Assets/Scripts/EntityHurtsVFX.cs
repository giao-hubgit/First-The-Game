using UnityEngine;
using System.Collections;

public class EntityHurtsVFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material originalM;

    [SerializeField] private Material onDamageVFX_Mat;
    [SerializeField] private float onDamageVFX_Duration = 0.15f;

    private Coroutine onDamageVFXCor;
    private WaitForSeconds waitDuration;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        originalM = sr.sharedMaterial;

        waitDuration = new WaitForSeconds(onDamageVFX_Duration);
    }

    public void PlayOnDamageVFX()
    {
        if (onDamageVFXCor != null)
        {
            StopCoroutine(onDamageVFXCor);
        }

        onDamageVFXCor = StartCoroutine(OnDamageVFXCo());
    }

    private IEnumerator OnDamageVFXCo()
    {
        sr.sharedMaterial = onDamageVFX_Mat;

        yield return waitDuration;

        sr.sharedMaterial = originalM;
        onDamageVFXCor = null;
    }

    private void OnDisable()
    {
        if (sr != null && originalM != null)
        {
            sr.sharedMaterial = originalM;
        }
    }
}