using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    [Header("Laser Configuration")]
    [SerializeField] private LaserData laserData;

    private BoxCollider2D laserCollider;
    private SpriteRenderer spriteRenderer;
    private float currentSweptAngle = 0f;
    private float nextDamageTime = 0f;

    private void Awake()
    {
        laserCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (laserCollider != null) laserCollider.isTrigger = true;
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }
    }

    private void OnEnable()
    {
        if (laserData == null) return;

        currentSweptAngle = 0f;
        nextDamageTime = 0f;

        transform.localScale = Vector3.one;

        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = new Vector2(laserData.length, laserData.width);
        }

        if (laserCollider != null)
        {
            laserCollider.size = new Vector2(laserData.length, laserData.width);

            laserCollider.offset = new Vector2(laserData.length / 2f, 0f);
        }

        SFXManager.Instance?.PlaySFX(laserData.laserSFX, transform.position);

        transform.Rotate(0f, 0f, laserData.startAngleOffset);
    }

    private void Update()
    {
        if (laserData == null) return;

        float angleToRotate = laserData.rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, 0f, angleToRotate);

        currentSweptAngle += Mathf.Abs(angleToRotate);

        if (currentSweptAngle >= laserData.rotationAngle)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (laserData == null) return;

        if (Time.time >= nextDamageTime)
        {
            if (Time.time >= nextDamageTime)
            {
                if (collision.TryGetComponent<IDamageable>(out IDamageable damageable) && !collision.gameObject.CompareTag("Enemy"))
                {
                    damageable.takeDmg(laserData.damage);
                }
            }
        }
    }
}