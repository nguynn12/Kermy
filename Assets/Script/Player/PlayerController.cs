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
    private bool _controlEnabled = true;
    private bool _jumpRequested;
    private readonly Collider2D[] _groundHits = new Collider2D[1];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
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
        float targetVelocityX = moveX * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // Jump.
        if (_jumpRequested)
        {
            _jumpRequested = false;
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
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

        int count = Physics2D.OverlapCircleNonAlloc(groundCheck.position, groundCheckRadius, _groundHits, groundLayer);
        isGrounded = count > 0;
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