using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Cài đặt Di chuyển & Nhảy")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Cài đặt Chạm đất")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Cài đặt Hạt Đất (Particles)")]
    public GameObject dirtPrefab; // Kéo thả file Prefab hạt đất vào đây
    public float spawnInterval = 0.1f; // Khoảng thời gian giữa các lần văng hạt đất
    private float particleTimer;
    
    // Tạo Object Pool đơn giản bằng Mảng (Array)
    private int poolSize = 15;
    private GameObject[] dirtPool;
    private int currentPoolIndex = 0;

    [Header("Input Actions (Kéo thả từ Input Map)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Runtime State")]
    public Vector2 MoveInput { get; private set; }
    public bool IsActionHeld { get; private set; }
    public bool IsInputEnabled => _inputEnabled;

    private bool _inputEnabled = true;
    private bool _jumpPressedThisFrame;
    private bool _actionPressedThisFrame;

    // Các biến dùng cho Di chuyển và Lật mặt
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isGrounded;

    private void Start()
    {
        // Lấy component vật lý của nhân vật
        rb = GetComponent<Rigidbody2D>();

        // KHỞI TẠO OBJECT POOL CHO HẠT ĐẤT
        // Game sẽ đẻ sẵn 15 hạt đất nhưng giấu đi, khi cần sẽ lôi ra dùng rồi cất lại
        dirtPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            if (dirtPrefab != null)
            {
                dirtPool[i] = Instantiate(dirtPrefab);
                dirtPool[i].SetActive(false); // Ẩn đi chờ ngày sử dụng
            }
        }
    }

    private void OnEnable()
    {
        BindAndEnable(moveAction, OnMove);
        BindAndEnable(jumpAction, OnJump);
        BindAndEnable(interactAction, OnInteract);

        ApplyInputEnabledState();
    }

    private void OnDisable()
    {
        UnbindAndDisable(moveAction, OnMove);
        UnbindAndDisable(jumpAction, OnJump);
        UnbindAndDisable(interactAction, OnInteract);
    }

    private void Update()
    {
        if (!_inputEnabled) return;

        // --- 1. KIỂM TRA CHẠM ĐẤT ---
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        // --- 2. DI CHUYỂN ---
        rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);

        // --- SPAWN HẠT ĐẤT KHI CHẠY ---
        // Nếu nhân vật đang chạy (có ấn phím) VÀ đang chạm đất
        if (Mathf.Abs(MoveInput.x) > 0.1f && isGrounded)
        {
            particleTimer -= Time.deltaTime;
            if (particleTimer <= 0f)
            {
                SpawnDirtFromPool(MoveInput.x);
                particleTimer = spawnInterval; // Reset thời gian chờ
            }
        }
        else
        {
            particleTimer = 0f; // Reset ngay khi dừng lại để chạy là văng hạt đất luôn
        }

        // --- 3. NHẢY ---
        if (ConsumeJumpPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Backward compatibility cũ của bạn
        if (jumpAction == null && moveAction != null)
        {
            if (MoveInput.y > 0.5f && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        // --- 4. LẬT MẶT KHI QUAY ĐẦU ---
        if (MoveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (MoveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    // --- HÀM LÔI HẠT ĐẤT RA TỪ POOL ĐỂ DÙNG ---
    private void SpawnDirtFromPool(float directionX)
    {
        if (dirtPrefab == null) return; // Nếu quên kéo thả Prefab trong Unity thì sẽ không chạy hàm này để tránh báo lỗi

        // Lấy hạt hiện tại trong mảng
        GameObject dirt = dirtPool[currentPoolIndex];
        
        // Gọi hàm Spawn trong script DirtParticle (script ở Bước 2)
        DirtParticle particleScript = dirt.GetComponent<DirtParticle>();
        if (particleScript != null && groundCheck != null)
        {
            // Cho hạt xuất hiện ở vị trí chân (groundCheck)
            particleScript.Spawn(groundCheck.position, directionX);
        }

        // Chuyển sang hạt tiếp theo trong Pool, nếu đến cuối thì vòng lại hạt số 0
        currentPoolIndex++;
        if (currentPoolIndex >= poolSize)
        {
            currentPoolIndex = 0;
        }
    }

    // Hàm lật mặt nhân vật
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // Hiển thị vòng tròn đỏ ở chân trong Editor để dễ canh chỉnh
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // ==========================================
    // CÁC HÀM XỬ LÝ INPUT CŨ CỦA BẠN (GIỮ NGUYÊN)
    // ==========================================

    public void SetInputEnabled(bool enabled)
    {
        if (_inputEnabled == enabled) return;
        _inputEnabled = enabled;
        ApplyInputEnabledState();
        if (!enabled)
        {
            MoveInput = Vector2.zero;
            IsActionHeld = false;
            _jumpPressedThisFrame = false;
            _actionPressedThisFrame = false;
        }
    }

    public bool ConsumeJumpPressed()
    {
        bool pressed = _jumpPressedThisFrame;
        _jumpPressedThisFrame = false;
        return pressed;
    }

    public bool ConsumeActionPressed()
    {
        bool pressed = _actionPressedThisFrame;
        _actionPressedThisFrame = false;
        return pressed;
    }

    private void ApplyInputEnabledState()
    {
        if (_inputEnabled)
        {
            EnableIfNotNull(moveAction);
            EnableIfNotNull(jumpAction);
            EnableIfNotNull(interactAction);
        }
        else
        {
            DisableIfNotNull(moveAction);
            DisableIfNotNull(jumpAction);
            DisableIfNotNull(interactAction);
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) { MoveInput = Vector2.zero; return; }
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) return;
        if (ctx.performed) _jumpPressedThisFrame = true;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) { IsActionHeld = false; return; }
        if (ctx.started) { IsActionHeld = true; _actionPressedThisFrame = true; }
        else if (ctx.canceled) { IsActionHeld = false; }
    }

    private static void BindAndEnable(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef == null || actionRef.action == null) return;
        actionRef.action.performed += callback;
        actionRef.action.canceled += callback;
        actionRef.action.started += callback;
        actionRef.action.Enable();
    }

    private static void UnbindAndDisable(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef == null || actionRef.action == null) return;
        actionRef.action.performed -= callback;
        actionRef.action.canceled -= callback;
        actionRef.action.started -= callback;
        actionRef.action.Disable();
    }

    private static void EnableIfNotNull(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null) actionRef.action.Enable();
    }

    private static void DisableIfNotNull(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null) actionRef.action.Disable();
    }
}