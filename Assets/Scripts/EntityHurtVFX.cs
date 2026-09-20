using System.Collections;
using UnityEngine;

public class EntityHurtVFX : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float onDamageVFX_Duration = 0.15f;

    private SpriteRenderer sr;
    private MaterialPropertyBlock propBlock;
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    private Coroutine onDamageVFXCor;
    private WaitForSeconds waitDuration;

    private Player player;
    private Enemy enemy;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        propBlock = new MaterialPropertyBlock();
        waitDuration = new WaitForSeconds(onDamageVFX_Duration);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        if (player != null) player.OnTakeDamage += PlayFlashVFX;
        if (enemy != null) enemy.OnTakeDamage += PlayFlashVFX;
    }

    private void OnDisable()
    {
        if (player != null) player.OnTakeDamage -= PlayFlashVFX;
        if (enemy != null) enemy.OnTakeDamage -= PlayFlashVFX;

        ResetFlash();
    }

    private void PlayFlashVFX()
    {
        if (sr == null) return;

        if (onDamageVFXCor != null)
        {
            StopCoroutine(onDamageVFXCor);
        }

        onDamageVFXCor = StartCoroutine(OnDamageVFXCo());
    }

    private IEnumerator OnDamageVFXCo()
    {
        sr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(FlashAmountID, 1f);
        sr.SetPropertyBlock(propBlock);

        yield return waitDuration;

        ResetFlash();
        onDamageVFXCor = null;
    }

    private void ResetFlash()
    {
        if (sr == null) return;
        sr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(FlashAmountID, 0f);
        sr.SetPropertyBlock(propBlock);
    }
}