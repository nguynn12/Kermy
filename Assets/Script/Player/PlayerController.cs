using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ground Detection")]
    public Transform groundCheck; 
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; 

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    
    private bool isGrounded;
    private bool wasJumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // 1. Kiểm tra chạm đất (có check null để tránh lỗi nếu quên kéo object vào)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 2. Xử lý nhảy (Trục Y của moveInput > 0 tức là đang bấm W hoặc Mũi tên lên)
        bool isPressingUp = inputHandler.moveInput.y > 0.5f;

        if (isPressingUp && isGrounded && !wasJumpPressed)
        {
            Jump();
            wasJumpPressed = true;
        }
        else if (!isPressingUp)
        {
            wasJumpPressed = false; 
        }
    }

    private void FixedUpdate()
    {
        // 3. Xử lý di chuyển ngang bằng vật lý
        float targetVelocityX = inputHandler.moveInput.x * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // Vẽ vòng tròn đỏ để debug điểm chạm đất trong Scene
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}