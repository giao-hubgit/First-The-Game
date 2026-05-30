using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour
{
    // ⭐ ĐỔI THÀNH LỚP CHA: Giờ bạn kéo thả RangedWeaponData hay MeleeWeaponData vào ô này đều được!
    public WeaponData weaponData;

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
            PlayerInteraction interaction = other.GetComponent<PlayerInteraction>();
            PlayerWeaponManager weaponManager = other.GetComponent<PlayerWeaponManager>();

            if (interaction != null && weaponManager != null)
            {
                if (weaponManager.IsSlotEmpty(weaponData.weaponType))
                {
                    Interact(other.gameObject);
                }
                else
                {
                    interaction.AddInteractableWeapon(this);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction interaction = other.GetComponent<PlayerInteraction>();
            if (interaction != null)
            {
                interaction.RemoveInteractableWeapon(this);
            }
        }
    }

    public void Interact(GameObject player)
    {
        PlayerWeaponManager weaponManager = player.GetComponent<PlayerWeaponManager>();

        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);

            SFXManager.Instance?.PlaySFX(chestSFX, transform.position);
            SpawnFloatingText();

            gameObject.SetActive(false);
        }
    }

    void SpawnFloatingText()
    {
        if (floatingTextPrefab != null && weaponData != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up;
            GameObject popup = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            FloatingText ftScript = popup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetText(weaponData.weaponName, Color.white);
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