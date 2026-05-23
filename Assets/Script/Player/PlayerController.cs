using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ladder Settings")]
    public float climbSpeed = 4f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask jumpSupportLayer;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private PlayerSupportDetector supportDetector;
    private PlayerRiderStick riderStick;

    private bool isGrounded;
    private bool isOnLadder;
    private float originalGravityScale;

    private bool _controlEnabled = true;
    private bool _jumpRequested;
    
    // Biến trung gian hứng hướng di chuyển từ Update xuống FixedUpdate
    private float _trackedMoveX;
    private float _trackedMoveY;
    
    // Biến phụ trợ bảo lãnh trạng thái đứng trên đất khi chạm bẫy sập
    private bool _isForcedGroundedByTrap;

    private readonly RaycastHit2D[] _groundRayHits = new RaycastHit2D[4];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        supportDetector = GetComponent<PlayerSupportDetector>();
        riderStick = GetComponent<PlayerRiderStick>();

        originalGravityScale = rb.gravityScale;
    }

    // ====================================================================
    // 🔥 ĐÃ SỬA KHU VỰC UPDATE: Bắt GetKey liên tục để chống nuốt phím leo thang
    // ====================================================================
    private void Update()
    {
        if (!_controlEnabled)
        {
            _trackedMoveX = 0f;
            _trackedMoveY = 0f;
            return;
        }

        string currentTag = gameObject.tag; // Lấy nhãn để phân chia chủ quyền

        // Hướng di chuyển gốc từ Handler
        if (inputHandler != null)
        {
            _trackedMoveX = inputHandler.MoveInput.x;
            _trackedMoveY = inputHandler.MoveInput.y;

            // Chấp nhận nút nhảy từ Handler phát ra
            if (inputHandler.ConsumeJumpPressed())
            {
                _jumpRequested = true;
            }
            
            // 🌟 CHỈ CHO PHÉP ẾCH NƯỚC (Player2) NHẢY BẰNG PHÍM MŨI TÊN LÊN
            if (currentTag == "Player2" && Input.GetKeyDown(KeyCode.UpArrow))
            {
                _jumpRequested = true;
            }
        }

        // ====================================================================
        // 🔥 XỬ LÝ TRÊN THANG: Ép dùng GetKey thời gian thực để leo siêu mượt
        // ====================================================================
        if (isOnLadder)
        {
            if (currentTag == "Player2") // Ếch Nước
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    _trackedMoveY = 1f;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    _trackedMoveY = -1f;
                }
                else
                {
                    _trackedMoveY = 0f; // Thả tay ra thì đứng im trên thang chứ không trôi
                }
            }
            else if (currentTag == "Player1") // Ếch Lửa
            {
                if (Input.GetKey(KeyCode.W))
                {
                    _trackedMoveY = 1f;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    _trackedMoveY = -1f;
                }
                else
                {
                    _trackedMoveY = 0f; // Thả tay ra thì đứng im trên thang chứ không trôi
                }
            }
        }
        else
        {
            // SỬA LỖI TRÔI: Khi ĐÃ RỜI THANG, nếu người chơi không bấm nút di chuyển dọc,
            // bắt buộc phải đưa _trackedMoveY về lại 0 để trả lại trọng lực rơi bình thường.
            if (inputHandler == null || inputHandler.MoveInput.y == 0f)
            {
                // Nếu là con Nước đang không bấm mũi tên dọc thì reset
                if (currentTag == "Player2" && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
                {
                    _trackedMoveY = 0f;
                }
                else if (currentTag == "Player1")
                {
                    _trackedMoveY = 0f;
                }
            }
        }
    }

    // ====================================================================
    // 🔥 ĐÃ SỬA KHU VỰC FIXEDUPDATE: Khóa cứng quán tính ngang khi leo thang
    // ====================================================================
    private void FixedUpdate()
    {
        UpdateGrounded();

        if (!_controlEnabled)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            _jumpRequested = false;
            return;
        }

        float moveX = _trackedMoveX;
        float moveY = _trackedMoveY;

        float baseVelocityX = 0f;

        if (riderStick != null && riderStick.ParentRigidbody != null)
        {
            baseVelocityX = riderStick.ParentRigidbody.linearVelocity.x;
        }

        float targetVelocityX = (moveX * moveSpeed) + baseVelocityX;
        
        // --- ĐOẠN SỬA GIA CỐ VẬT LÝ THANG ---
        if (isOnLadder)
        {
            targetVelocityX = 0f;
            rb.gravityScale = 0f;
            // Ép hẳn vận tốc ngang về 0 và leo lên mượt mà bằng Velocity dọc, dẹp bỏ lực AddForce phản chủ
            rb.linearVelocity = new Vector2(0f, moveY * climbSpeed); 
        }
        else
        {
            rb.gravityScale = originalGravityScale;
            
            // Chỉ tính toán và bơm lực AddForce di chuyển ngang khi ĐANG KHÔNG LEO THANG
            float forceX = (targetVelocityX - rb.linearVelocity.x) * rb.mass / Time.fixedDeltaTime;
            rb.AddForce(new Vector2(forceX, 0f));
        }

        if (_jumpRequested)
        {
            // Kiểm tra điều kiện nhảy: Chạm đất thông thường, đứng trên thang, hoặc được bẫy sập bảo lãnh
            if ((isGrounded || _isForcedGroundedByTrap || isOnLadder) && (supportDetector == null || !supportDetector.IsSupportingPlayer))
            {
                Jump();
            }
            
            _jumpRequested = false;
        }
    }

    private void Jump()
    {
        if (supportDetector != null && supportDetector.IsSupportingPlayer)
            return;

        if (isOnLadder)
        {
            isOnLadder = false;
            rb.gravityScale = originalGravityScale;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ApplyBounce(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
    }

    public void SetMovementEnabled(bool enabled)
    {
        if (_controlEnabled == enabled)
            return;

        _controlEnabled = enabled;

        if (!enabled)
        {
            _jumpRequested = false;
            _trackedMoveX = 0f;
            _trackedMoveY = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetControlEnabled(bool enabled)
    {
        if (_controlEnabled == enabled)
            return;

        SetMovementEnabled(enabled);

        if (inputHandler != null)
        {
            inputHandler.SetInputEnabled(enabled);
        }
    }

    private void UpdateGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        LayerMask mask = jumpSupportLayer.value != 0 ? jumpSupportLayer : groundLayer;
        float rayDistance = Mathf.Max(0.01f, groundCheckRadius + 0.05f);

        int hitCount = Physics2D.RaycastNonAlloc(groundCheck.position, Vector2.down, _groundRayHits, rayDistance, mask);

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = _groundRayHits[i];

            if (hit.collider == null)
                continue;

            if (hit.rigidbody == rb)
                continue;

            if (hit.normal.y < 0.5f)
                continue;

            isGrounded = true;
            return;
        }

        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            rb.gravityScale = originalGravityScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // ====================================================================
    // 🔥 CÁC HÀM PHỤ TRỢ ĐỂ KẾT NỐI VỚI TRAPPLATFORM KHÔNG BỊ NUỐT PHÍM
    // ====================================================================

    // Bẫy sập gọi hàm này để ép con ếch luôn nhảy được khi đứng lên bẫy
    public void ForceGroundedFromTrap(bool grounded)
    {
        _isForcedGroundedByTrap = grounded;
    }

    // Bẫy sập check xem người chơi có đang bấm nút nhảy ở Update không
    public bool IsJumpRequested => _jumpRequested;
}