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
    public LayerMask jumpSupportLayer;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private PlayerSupportDetector supportDetector;
    private PlayerRiderStick riderStick;

    private bool isGrounded;
    private bool _controlEnabled = true;
    private bool _jumpRequested;
    private readonly RaycastHit2D[] _groundRayHits = new RaycastHit2D[4];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        supportDetector = GetComponent<PlayerSupportDetector>();
        riderStick = GetComponent<PlayerRiderStick>();
    }

    private void Update()
    {
        // Input should be sampled in Update. Physics should be applied in FixedUpdate.
        if (!_controlEnabled)
        {
            return;
        }

        // Queue a jump request. We resolve whether it can actually jump inside FixedUpdate
        // after updating ground state.
        if (inputHandler != null && inputHandler.ConsumeJumpPressed())
        {
            _jumpRequested = true;
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

        // Horizontal movement.
        float moveX = inputHandler != null ? inputHandler.MoveInput.x : 0f;
        float baseVelocityX = 0f;
        // Nếu mình đang cưỡi trên đầu ai đó, hãy lấy vận tốc của người đó làm gốc
        if (riderStick != null && riderStick.ParentRigidbody != null)
        {
            baseVelocityX = riderStick.ParentRigidbody.linearVelocity.x;
        }
        
        float targetVelocityX = (moveX * moveSpeed) + baseVelocityX;
        
        float forceX = (targetVelocityX - rb.linearVelocity.x) * rb.mass / Time.fixedDeltaTime;
        rb.AddForce(new Vector2(forceX, 0f));

        // Jump.
        if (_jumpRequested)
        {
            _jumpRequested = false;
            if (isGrounded && (supportDetector == null || !supportDetector.IsSupportingPlayer))
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        // Nếu có người đứng trên đầu thì KHÔNG cho nhảy!
        if (supportDetector != null && supportDetector.IsSupportingPlayer)
        {
            return; 
        }
        // Reset vertical velocity to keep jump height consistent.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ApplyBounce(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
    }

    public void SetControlEnabled(bool enabled)
    {
        if (_controlEnabled == enabled)
        {
            return;
        }

        _controlEnabled = enabled;
        if (inputHandler != null)
        {
            inputHandler.SetInputEnabled(enabled);
        }

        if (!enabled)
        {
            _jumpRequested = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateGrounded()
    {
        // Check ground in FixedUpdate to align with physics.
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
            {
                continue;
            }

            if (hit.rigidbody == rb)
            {
                continue;
            }

            if (hit.normal.y < 0.5f)
            {
                continue;
            }

            isGrounded = true;
            return;
        }

        isGrounded = false;
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