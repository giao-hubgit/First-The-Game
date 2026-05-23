using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;
    public GameObject floatingTextPrefab;
    public AudioClip chestSFX;

    [SerializeField] private Material outlineMaterial;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapon pw = other.GetComponentInChildren<PlayerWeapon>();
            if (pw != null)
            {
                pw.SetInteractableWeapon(this);
            }

            if (outlineMaterial != null && spriteRenderer != null)
            {
                spriteRenderer.material = outlineMaterial;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapon pw = other.GetComponentInChildren<PlayerWeapon>();
            if (pw != null)
            {
                pw.SetInteractableWeapon(null);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.material = originalMaterial;
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
            GameObject popup = Instantiate(floatingTextPrefab, Vector3.up, Quaternion.identity);

            FloatingText ftScript = popup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetText(weaponToGive.weaponName, Color.white);
            }
        }
    }

    private void OnEnable()
    {
        if (spriteRenderer != null && originalMaterial != null)
        {
            spriteRenderer.material = originalMaterial;
        }
    }
}