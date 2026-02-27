using UnityEngine;
using System.Collections;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material originalM;

    [SerializeField] private Material onDamageVFXmat;
    [SerializeField] private float onDamageVFXDuration = 0.15f; 
    private Coroutine OnDamageVFXCor;

    private void Awake(){
        sr = GetComponent<SpriteRenderer>();
        originalM = sr.material;
    }

    public void PlayOnDamageVFX(){

        OnDamageVFXCor = StartCoroutine(OnDamageVFXCo());
    }

    private IEnumerator OnDamageVFXCo(){
        sr.material = onDamageVFXmat;

        yield return new WaitForSeconds(onDamageVFXDuration);

        sr.material = originalM;
    }
}
