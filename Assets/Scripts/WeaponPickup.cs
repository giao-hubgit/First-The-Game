using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponData weaponData;

    [Header("Runtime Dynamic Data")]
    public int currentAmmo;

    [Header("Components & Effects")]
    public GameObject floatingTextPrefab;
    public AudioClip weaponPickupSFX;
    public Light2D hightLight;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (hightLight == null)
            hightLight = GetComponentInChildren<Light2D>();

        if (hightLight != null)
        {
            hightLight.enabled = false;
            if (weaponData != null)
            {
                hightLight.color = weaponData.outlineColor;
            }
        }

        if (spriteRenderer != null && weaponData != null)
        {
            spriteRenderer.sprite = weaponData.weaponIcon;
        }

        if (weaponData != null && weaponData.weaponType == WeaponType.Ranged)
        {
            WeaponRangedData rangedData = weaponData as WeaponRangedData;
            currentAmmo = rangedData.maxAmmo;
        }
    }

    public void InitPickup(WeaponData data, int ammo = -1)
    {
        weaponData = data;

        if (ammo >= 0)
        {
            currentAmmo = ammo;
        }
        else if (data is WeaponRangedData rangedData)
        {
            currentAmmo = rangedData.maxAmmo;
        }
        else
        {
            currentAmmo = 0;
        }

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && weaponData != null)
        {
            spriteRenderer.sprite = weaponData.weaponIcon;
        }

        if (hightLight != null && weaponData != null)
        {
            hightLight.color = weaponData.outlineColor;
        }

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
            weaponManager.EquipWeapon(weaponData, currentAmmo);

            SFXManager.Instance?.PlaySFX(weaponPickupSFX, transform.position);
            SpawnFloatingText();

            ResetData();
            gameObject.SetActive(false);
        }
    }

    public void ResetData()
    {
        weaponData = null;
        currentAmmo = 0;
        if (spriteRenderer != null) spriteRenderer.sprite = null;
        SetHighlight(false);
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (hightLight == null) return;
        hightLight.enabled = isHighlighted;
    }

    private void OnDisable()
    {
        ResetData();
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
                Color textColor = weaponData != null ? weaponData.outlineColor : Color.white;
                ftScript.SetText(weaponData.weaponName, textColor);
            }
        }
    }
}