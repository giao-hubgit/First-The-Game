using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpd = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    public TrailRenderer tr;


    private bool canDash = true;
    public bool isDashing;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCD = 1f;
    public int dashDMG = 20;

    Vector2 movement;
    Vector2 mousePos;

    void Awake() {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }
    
    // Hàm nhận tín hiệu di chuyển từ Player Input Component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Đọc giá trị Vector2 
        movement = context.ReadValue<Vector2>();
    }

    // Hàm nhận tín hiệu vị trí chuột từ Player Input Component
    public void OnLook(InputAction.CallbackContext context)
    {
        // Đọc vị trí chuột trên màn hình
        Vector2 screenMousePos = context.ReadValue<Vector2>();
        
        // Chuyển đổi vị trí chuột từ màn hình sang tọa độ
        if (cam != null)
        {
            mousePos = cam.ScreenToWorldPoint(screenMousePos);
        }
    }

    public void onDash(InputAction.CallbackContext context){
        if (canDash && context.performed){
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash(){
        Vector2 dashDir = (mousePos - rb.position).normalized;
        canDash = false;
        isDashing = true;
        rb.linearVelocity = dashDir * dashPower;
        if (tr != null) tr.emitting = true;
        yield return new WaitForSeconds(dashTime);

        if (tr != null) tr.emitting = false;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }

    public void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Enemy")){
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null && isDashing){
                enemy.takeDmg(dashDMG);
            }
        }
    }

    void FixedUpdate()
    {
        // Đang dash thì không di chuyển
        if (isDashing) return;

        // Di chuyển Player
        rb.MovePosition(rb.position + movement * moveSpd * Time.fixedDeltaTime);

        // Xoay nhân vật hướng về phía chuột
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        
        rb.MoveRotation(angle);
    }
}