using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponData weaponData;

    public GameObject floatingTextPrefab;
    public AudioClip weaponPickupSFX;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();

        SetHighlight(false);
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
                    interaction.AddInteractable(this);
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
                interaction.RemoveInteractable(this);
            }
        }
    }

    public void Interact(GameObject player)
    {
        PlayerWeaponManager weaponManager = player.GetComponent<PlayerWeaponManager>();

        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);

            SFXManager.Instance?.PlaySFX(weaponPickupSFX, transform.position);
            SpawnFloatingText();

            gameObject.SetActive(false);
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (spriteRenderer == null) return;

        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_EnableOutline", isHighlighted ? 1f : 0f);
        spriteRenderer.SetPropertyBlock(propBlock);
    }

    private void OnEnable()
    {
        SetHighlight(false);
    }

    private void OnDisable()
    {
        SetHighlight(false);
    }

    private void SpawnFloatingText()
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
}