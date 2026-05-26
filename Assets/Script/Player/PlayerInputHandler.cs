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
    public GameObject dirtPrefab; // Kéo thả file Prefab hạt đất/lửa/nước vào đây
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
    private int _actionPressedFrame = -1;

    // Các biến dùng cho Di chuyển và Lật mặt
    private Rigidbody2D rb;
    private Animator anim; 
    private PlayerController playerController;
    private bool isFacingRight = true;
    private bool isGrounded;

    private void Start()
    {
        // Lấy component vật lý và animation của nhân vật
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        playerController = GetComponent<PlayerController>();

        // KHỞI TẠO OBJECT POOL CHO HIỆU ỨNG VĂNG HẠT
        dirtPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            if (dirtPrefab != null)
            {
                dirtPool[i] = Instantiate(dirtPrefab);
                dirtPool[i].SetActive(false); 
            }
        }
    }

    private void OnEnable()
    {
        ApplySavedBindingOverrides(moveAction);
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
        if (playerController != null)
        {
            isGrounded = playerController.IsGrounded;
        }
        else if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        // --- 2. LỰC DI CHUYỂN VẬT LÝ VÀ CHẠY ---
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        // --- 3. SPAWN HẠT KHI CHẠY ---
        if (Mathf.Abs(MoveInput.x) > 0.1f && isGrounded)
        {
            particleTimer -= Time.deltaTime;
            if (particleTimer <= 0f)
            {
                SpawnDirtFromPool(MoveInput.x);
                particleTimer = spawnInterval; 
            }
        }
        else
        {
            particleTimer = 0f; 
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

        // --- 5. ANIMATION: CẬP NHẬT TRẠNG THÁI CHO ANIMATOR ---
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(MoveInput.x) > 0.1f;
            anim.SetBool("isRunning", isMoving);
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    // --- HÀM LÔI HẠT RA TỪ POOL ĐỂ DÙNG ---
    private void SpawnDirtFromPool(float directionX)
    {
        if (dirtPrefab == null) return; 

        GameObject dirt = dirtPool[currentPoolIndex];
        
        DirtParticle particleScript = dirt.GetComponent<DirtParticle>();
        if (particleScript != null && groundCheck != null)
        {
            particleScript.Spawn(groundCheck.position, directionX);
        }

        currentPoolIndex++;
        if (currentPoolIndex >= poolSize)
        {
            currentPoolIndex = 0;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // ==========================================
    // CÁC HÀM XỬ LÝ INPUT (ĐÃ ĐƯỢC FIX LỖI TRÙNG PHÍM)
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
        bool pressed = _actionPressedThisFrame && _actionPressedFrame == Time.frameCount;
        _actionPressedThisFrame = false;
        return pressed;
    }

    public void ClearActionPressed()
    {
        _actionPressedThisFrame = false;
        _actionPressedFrame = -1;
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
        
        Vector2 rawInput = ctx.ReadValue<Vector2>();
        string currentTag = gameObject.tag;

        // ❌ CHẶN TRÙNG DI CHUYỂN: Con Lửa (Player1) chỉ nhận phím chữ, KHÔNG nhận phím mũi tên
        if (currentTag == "Player1" && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            MoveInput = Vector2.zero;
            return;
        }

        // ❌ CHẶN TRÙNG DI CHUYỂN: Con Nước (Player2) chỉ nhận phím mũi tên, KHÔNG nhận phím A/D
        if (currentTag == "Player2" && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = rawInput;
    }

   private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) return;
        
        if (ctx.performed)
        {
            string currentTag = gameObject.tag;

            // 🌟 ĐOẠN KIỂM TRA CHUẨN CỦA INPUT SYSTEM:
            // Lấy tên của nút bấm thực tế vừa được nhấn từ bàn phím
            string activeKeyName = ctx.control.name; // Nó sẽ trả về chữ "w" hoặc "upArrow"

            // 1. Nếu đây là con Lửa (Player1) nhưng nút vừa bấm lại là Mũi tên lên (upArrow) -> CHẶN
            if (currentTag == "Player1" && activeKeyName.ToLower().Contains("arrow"))
            {
                return; 
            }

            // 2. Nếu đây là con Nước (Player2) nhưng nút vừa bấm lại là phím W -> CHẶN
            if (currentTag == "Player2" && activeKeyName.ToLower() == "w")
            {
                return; 
            }

            // ---- NẾU ĐÚNG CHỦ QUYỀN THÌ MỚI CHO NHẢY ----
            _jumpPressedThisFrame = true;

            if (isGrounded && rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) { IsActionHeld = false; return; }
        if (ctx.started)
        {
            IsActionHeld = true;
            _actionPressedThisFrame = true;
            _actionPressedFrame = Time.frameCount;
        }
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

    private static void ApplySavedBindingOverrides(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null || actionRef.action.actionMap == null)
        {
            return;
        }

        KeybindingManager.ApplySavedOverrides(actionRef.action.actionMap.asset);
    }
}
