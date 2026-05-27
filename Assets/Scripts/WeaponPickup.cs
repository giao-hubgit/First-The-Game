using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;
    public GameObject floatingTextPrefab;
    public AudioClip chestSFX;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();

        ToggleOutline(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapon pw = other.GetComponent<PlayerWeapon>();
            if (pw != null)
            {
                if (pw.currentWeapon != null && pw.currentWeapon != pw.nullWeapon)
                {
                    pw.AddInteractableWeapon(this);
                }
                else
                {
                    Interact(pw);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapon pw = other.GetComponent<PlayerWeapon>();
            if (pw != null)
            {
                pw.RemoveInteractableWeapon(this);
            }
        }
    }

    public void Interact(PlayerWeapon playerWeapon)
    {
        playerWeapon.ChangeWeapon(weaponToGive);
        SFXManager.Instance?.PlaySFX(chestSFX, transform.position);
        SpawnFloatingText();

        gameObject.SetActive(false);
    }

    void SpawnFloatingText()
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up;
            GameObject popup = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            FloatingText ftScript = popup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetText(weaponToGive.weaponName, Color.white);
            }
        }
    }

    private void OnEnable()
    {
        ToggleOutline(false);
    }

    private void OnDisable()
    {
        ToggleOutline(false);
    }

    public void ToggleOutline(bool isActive)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Thickness", isActive ? 2f : 0f);
        spriteRenderer.SetPropertyBlock(propBlock);
    }
}