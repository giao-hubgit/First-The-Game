using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class PlayerRangedWeapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private GameObject weaponUIContainer;
    [SerializeField] private Image weaponIconUI;
    [SerializeField] private TextMeshProUGUI ammoTextUI;
    [SerializeField] private AudioClip outOfAmmoSFX;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private IAttacker ownerAttacker;
    [SerializeField] private PlayerMovement pm;

    public WeaponRangedData currentWeapon;
    public WeaponRangedData nullWeapon;

    private int currentAmmo;
    private float nextFireTime = 0f;
    public bool isFiring = false;

    public RectTransform hudContainerTransform;
    private Coroutine refreshCoroutine;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        pm = GetComponent<PlayerMovement>();
        ownerAttacker = GetComponentInParent<IAttacker>();
    }

    void Start()
    {
        if (currentWeapon != null) currentAmmo = currentWeapon.maxAmmo;
        UpdateWeaponUI();
    }

    public void Equip(WeaponRangedData newWeapon)
    {
        currentWeapon = newWeapon;
        currentAmmo = currentWeapon != null ? currentWeapon.maxAmmo : 0;
        nextFireTime = 0f;
        UpdateWeaponUI();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (currentWeapon == null || currentWeapon == nullWeapon) return;

        if (currentWeapon.isAutomatic)
        {
            if (context.started || context.performed) isFiring = true;
            if (context.canceled) isFiring = false;
        }
        else
        {
            isFiring = false;
            if (context.performed) Shoot();
        }
    }

    void Update()
    {
        if (isFiring && currentWeapon != null && currentWeapon.isAutomatic)
        {
            if (Time.unscaledTime >= nextFireTime) Shoot();
        }
    }

    void Shoot()
    {
        if (currentWeapon == null || currentWeapon == nullWeapon || currentAmmo <= 0) return;
        if (Time.unscaledTime < nextFireTime) return;

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
        }

        for (int i = 0; i < currentWeapon.burstCount; i++)
        {
            float randomSpread = Random.Range(-currentWeapon.spread, currentWeapon.spread);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

            GameObject bullet = ObjectPooler.Instance.SpawnFromPool(
                currentWeapon.bulletTag,
                firePoint.position,
                bulletRotation
            );

            if (bullet != null)
            {
                // Laser
                if (bullet.TryGetComponent<Laser>(out Laser laser))
                {
                    bullet.transform.SetParent(firePoint, false);
                    bullet.transform.localPosition = Vector3.zero;
                    pm.isLocked = true;
                    float fadeDuration = laser.laserData.fadeDuration;
                    float sweepDuration = Mathf.Abs(laser.laserData.endAngleOffset - laser.laserData.startAngleOffset) / laser.laserData.rotationSpeed;
                    StartCoroutine(StopRotating(sweepDuration + fadeDuration));
                }
                else // Bullet
                {
                    if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
                    {
                        bulletScript.Init(ownerAttacker);
                    }

                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = bullet.transform.up * currentWeapon.bulletForce;
                    }
                }
            }
        }

        CameraShakeManager.Instance?.CameraShake(impulseSource, currentWeapon.shakeForce);
        SFXManager.Instance?.PlaySFX(currentWeapon.shootSFX, transform.position);

        nextFireTime = Time.unscaledTime + currentWeapon.fireRate;
        currentAmmo--;
        UpdateWeaponUI();

        if (currentAmmo <= 0) OutOfAmmoLogic();
    }

    private IEnumerator StopRotating(float timer)
    {
        yield return new WaitForSeconds(timer);
        pm.isLocked = false;
    }

    private void OutOfAmmoLogic()
    {
        isFiring = false;
        SFXManager.Instance?.PlaySFX(outOfAmmoSFX, transform.position);
        Equip(nullWeapon);
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon != null && currentWeapon != nullWeapon)
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(true);
            if (weaponIconUI != null) { weaponIconUI.sprite = currentWeapon.weaponIcon; weaponIconUI.enabled = true; }
            if (ammoTextUI != null) { ammoTextUI.text = $"{currentAmmo:00}/{currentWeapon.maxAmmo:00}"; ammoTextUI.enabled = true; }
        }
        else
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(false);
            if (weaponIconUI != null) weaponIconUI.enabled = false;
            if (ammoTextUI != null) ammoTextUI.enabled = false;
        }

        ForceInstantLayoutRefresh();
    }

    private void ForceInstantLayoutRefresh()
    {
        if (refreshCoroutine != null) StopCoroutine(refreshCoroutine);

        refreshCoroutine = StartCoroutine(RefreshLayoutRoutine());
    }

    private IEnumerator RefreshLayoutRoutine()
    {
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();

        if (hudContainerTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(hudContainerTransform);
        }
    }
}