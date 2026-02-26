using UnityEngine;
using UnityEngine.InputSystem;

public class playerShooting : MonoBehaviour
{
public Transform firePoint;
public GameObject bulletPrefab;

public float bulletForce = 20f;

    // Nhận nút
    public void onFire(InputAction.CallbackContext context)
    {
        if (context.performed){
            Shoot();
        }
    }
    
    // Bắn
    void Shoot(){
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
