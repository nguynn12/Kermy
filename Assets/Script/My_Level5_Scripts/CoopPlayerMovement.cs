using UnityEngine;

public class CoopPlayerMovement : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Cấu hình nút cho từng Player")]
    public KeyCode moveLeftKey;   // Phím đi qua trái
    public KeyCode moveRightKey;  // Phím đi qua phải
    public KeyCode jumpKey;       // Phím nhảy

    [Header("Kiểm tra mặt đất")]
    public Transform groundCheck; // Điểm kiểm tra dưới chân nhân vật
    public LayerMask groundLayer; // Layer của mặt đất để nhận diện va chạm
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private float horizontalInput = 0f;
    private bool isGrounded;

    void Start()
    {
        // Lấy component Rigidbody2D đã gắn trên nhân vật
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Reset lại input mỗi khung hình
        horizontalInput = 0f;

        Vector2 configuredMove = KeybindingManager.GetMoveInputForTag(gameObject.tag);
        if (configuredMove != Vector2.zero)
        {
            horizontalInput = configuredMove.x;
        }
        else
        {
            // Nhận phím di chuyển sang trái hoặc phải
            if (Input.GetKey(moveLeftKey)) horizontalInput = -1f;
            if (Input.GetKey(moveRightKey)) horizontalInput = 1f;
        }

        // Quay mặt Sprite nhân vật theo hướng di chuyển (Flip)
        if (horizontalInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Kiểm tra xem vòng tròn dưới chân có chạm vào mặt đất (Layer Ground) không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Xử lý lệnh nhảy khi nhấn phím và nhân vật đang đứng trên đất
        bool jumpPressed = KeybindingManager.GetJumpDownForTag(gameObject.tag)
            || (jumpKey != KeyCode.None && Input.GetKeyDown(jumpKey));
        if (jumpPressed && isGrounded)
        {
            // Mẹo Unity 6: Sử dụng 'linearVelocity' thay cho 'velocity' cũ
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Cập nhật vận tốc di chuyển mượt mà theo cơ chế vật lý 2D
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // Vẽ một vòng tròn đỏ trong màn hình Scene để em dễ căn chỉnh điểm kiểm tra đất
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
