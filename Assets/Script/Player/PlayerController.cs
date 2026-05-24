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

    [Header("Slope Handling")]
    [SerializeField] private float maxGroundedUpwardVelocity = 2f;
    [SerializeField] private float jumpVelocityClampDelay = 0.12f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private PlayerSupportDetector supportDetector;
    private PlayerRiderStick riderStick;

    private bool isGrounded;
    private bool isOnLadder;
    private float originalGravityScale;
    private float _ignoreGroundClampUntil;

    private bool _controlEnabled = true;
    private bool _jumpRequested;
    private readonly RaycastHit2D[] _groundRayHits = new RaycastHit2D[4];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        supportDetector = GetComponent<PlayerSupportDetector>();
        riderStick = GetComponent<PlayerRiderStick>();

        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (!_controlEnabled)
            return;

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

        float moveX = inputHandler != null ? inputHandler.MoveInput.x : 0f;
        float moveY = inputHandler != null ? inputHandler.MoveInput.y : 0f;

        float baseVelocityX = 0f;

        if (riderStick != null && riderStick.ParentRigidbody != null)
        {
            baseVelocityX = riderStick.ParentRigidbody.linearVelocity.x;
        }

        float targetVelocityX = (moveX * moveSpeed) + baseVelocityX;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        if (isOnLadder)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveY * climbSpeed);
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }

        if (_jumpRequested)
        {
            _jumpRequested = false;

            if ((isGrounded || isOnLadder) && (supportDetector == null || !supportDetector.IsSupportingPlayer))
            {
                Jump();
            }
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
        _ignoreGroundClampUntil = Time.time + jumpVelocityClampDelay;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ApplyBounce(float force)
    {
        _ignoreGroundClampUntil = Time.time + jumpVelocityClampDelay;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < _ignoreGroundClampUntil || isOnLadder)
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (contact.normal.y < 0.5f)
            {
                continue;
            }

            if (rb.linearVelocity.y > maxGroundedUpwardVelocity)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxGroundedUpwardVelocity);
            }

            return;
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
}
