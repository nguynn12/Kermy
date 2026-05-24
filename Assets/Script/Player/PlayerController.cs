using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(AudioSource))] // Bảo đảm Object bắt buộc phải có loa
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

    // ====================================================================
    // 🔥 KHU VỰC AUDIO ĐÃ KẾT NỐI: Mạnh thả file tương ứng vào đây ngoài Unity nha
    // ====================================================================
    [Header("Audio Settings")]
    public AudioSource audioSource; // Cái loa của con ếch
    public AudioClip jumpSound;      // Thả file Jump2.wav vào đây
    public AudioClip walkSound;      // Thả file Mutant.wav vào đây
    public AudioClip collectSound;   // Thả file Pickup4.wav vào đây
    public AudioClip deathSound;     // Ô MỚI: Thả file âm thanh lúc chết vào đây

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private PlayerSupportDetector supportDetector;
    private PlayerRiderStick riderStick;

    private bool isGrounded;
    private bool isOnLadder;
    private float originalGravityScale;

    private bool _controlEnabled = true;
    private bool _jumpRequested;
    
    private float _trackedMoveX;
    private float _trackedMoveY;
    
    private bool _isForcedGroundedByTrap;

    private readonly RaycastHit2D[] _groundRayHits = new RaycastHit2D[4];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        supportDetector = GetComponent<PlayerSupportDetector>();
        riderStick = GetComponent<PlayerRiderStick>();
        
        // Tự động tìm cái loa gắn trên con ếch
        audioSource = GetComponent<AudioSource>();

        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (!_controlEnabled)
        {
            _trackedMoveX = 0f;
            _trackedMoveY = 0f;
            return;
        }

        string currentTag = gameObject.tag; 

        if (inputHandler != null)
        {
            _trackedMoveX = inputHandler.MoveInput.x;
            _trackedMoveY = inputHandler.MoveInput.y;

            if (inputHandler.ConsumeJumpPressed())
            {
                _jumpRequested = true;
            }
            
            if (currentTag == "Player2" && Input.GetKeyDown(KeyCode.UpArrow))
            {
                _jumpRequested = true;
            }
        }

        if (isOnLadder)
        {
            if (currentTag == "Player2") 
            {
                if (Input.GetKey(KeyCode.UpArrow)) _trackedMoveY = 1f;
                else if (Input.GetKey(KeyCode.DownArrow)) _trackedMoveY = -1f;
                else _trackedMoveY = 0f;
            }
            else if (currentTag == "Player1") 
            {
                if (Input.GetKey(KeyCode.W)) _trackedMoveY = 1f;
                else if (Input.GetKey(KeyCode.S)) _trackedMoveY = -1f;
                else _trackedMoveY = 0f;
            }
        }
        else
        {
            if (inputHandler == null || inputHandler.MoveInput.y == 0f)
            {
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

        // ====================================================================
        // 🔊 XỬ LÝ TIẾNG CHẠY (MUTANT.WAV): Phát liên tục khi di chuyển trên đất
        // ====================================================================
        if (isGrounded && !isOnLadder && Mathf.Abs(_trackedMoveX) > 0.1f)
        {
            // Chỉ phát khi loa đang rảnh để bước chân không bị lặp đè dính cục chói tai
            if (audioSource != null && walkSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walkSound, 0.4f); // Chạy âm lượng 40% cho vừa tai
            }
        }
    }

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
        
        if (isOnLadder)
        {
            targetVelocityX = 0f;
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0f, moveY * climbSpeed); 
        }
        else
        {
            rb.gravityScale = originalGravityScale;
            float forceX = (targetVelocityX - rb.linearVelocity.x) * rb.mass / Time.fixedDeltaTime;
            rb.AddForce(new Vector2(forceX, 0f));
        }

        if (_jumpRequested)
        {
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

        // ====================================================================
        // 🔊 XỬ LÝ TIẾNG NHẢY (JUMP2.WAV): Cất cánh là búng kêu ngay!
        // ====================================================================
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound, 0.8f);
        }
    }

    // ====================================================================
    // 🔊 XỬ LÝ TIẾNG ĂN NGỌC (PICKUP4.WAV): Hàm mở sẵn cho script Ngọc gọi sang
    // ====================================================================
    public void PlayCollectSound()
    {
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound, 0.7f);
        }
    }

    // ====================================================================
    // 🔊 XỬ LÝ TIẾNG CHẾT: Hàm mở sẵn để script Bẫy gai/Nước độc gọi sang khi ếch ngỏm
    // ====================================================================
    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, 0.8f); // Phát tiếng chết âm lượng 80%
        }
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
            if (hit.collider == null || hit.rigidbody == rb || hit.normal.y < 0.5f) continue;

            isGrounded = true;
            return;
        }

        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) isOnLadder = true;
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

    public void ForceGroundedFromTrap(bool grounded) => _isForcedGroundedByTrap = grounded;
    public bool IsJumpRequested => _jumpRequested;
}