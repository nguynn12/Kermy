using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Dirt Particles")]
    public GameObject dirtPrefab;
    public float spawnInterval = 0.1f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Runtime State")]
    public Vector2 MoveInput { get; private set; }
    public bool IsActionHeld { get; private set; }
    public bool IsInputEnabled => _inputEnabled;

    private const int PoolSize = 15;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController playerController;
    private GameObject[] dirtPool;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool _inputEnabled = true;
    private bool _jumpPressedThisFrame;
    private bool _actionPressedThisFrame;
    private bool _usingKeyboardMoveFallback;
    private int currentPoolIndex;
    private int _actionPressedFrame = -1;
    private float particleTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        dirtPool = new GameObject[PoolSize];
        for (int i = 0; i < dirtPool.Length; i++)
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
        if (!_inputEnabled)
        {
            ApplyIdleAnimatorState();
            return;
        }

        ApplyKeyboardMoveFallback();
        ApplyKeyboardActionFallback();
        UpdateGroundedState();
        ApplyMovement();
        UpdateDirtParticles();
        UpdateFacingDirection();
        UpdateAnimator();
    }

    public void SetInputEnabled(bool enabled)
    {
        if (_inputEnabled == enabled)
        {
            return;
        }

        _inputEnabled = enabled;
        ApplyInputEnabledState();
        if (!enabled)
        {
            MoveInput = Vector2.zero;
            IsActionHeld = false;
            _jumpPressedThisFrame = false;
            _actionPressedThisFrame = false;
            _actionPressedFrame = -1;
            particleTimer = 0f;
            ApplyIdleAnimatorState();
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

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled)
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled || !ctx.performed)
        {
            return;
        }

        _jumpPressedThisFrame = true;
        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled)
        {
            IsActionHeld = false;
            return;
        }

        if (!IsAllowedActionControl(ctx))
        {
            return;
        }

        if (ctx.started)
        {
            IsActionHeld = true;
            _actionPressedThisFrame = true;
            _actionPressedFrame = Time.frameCount;
        }
        else if (ctx.canceled)
        {
            IsActionHeld = false;
        }
    }

    private void ApplyKeyboardMoveFallback()
    {
        Vector2 fallbackMove = KeybindingManager.GetMoveInputForTag(gameObject.tag);
        if (Mathf.Abs(fallbackMove.x) > 0.01f || Mathf.Abs(fallbackMove.y) > 0.01f)
        {
            MoveInput = fallbackMove;
            _usingKeyboardMoveFallback = true;
        }
        else if (_usingKeyboardMoveFallback)
        {
            MoveInput = Vector2.zero;
            _usingKeyboardMoveFallback = false;
        }
    }

    private void ApplyKeyboardActionFallback()
    {
        if (KeybindingManager.GetActionDownForTag(gameObject.tag))
        {
            IsActionHeld = true;
            _actionPressedThisFrame = true;
            _actionPressedFrame = Time.frameCount;
        }
        else if (KeybindingManager.GetActionUpForTag(gameObject.tag))
        {
            IsActionHeld = false;
        }
    }

    private bool IsAllowedActionControl(InputAction.CallbackContext ctx)
    {
        if (Keyboard.current == null || ctx.control == null || ctx.control.device != Keyboard.current)
        {
            return false;
        }

        return KeybindingManager.IsActionControlForTag(gameObject.tag, ctx.control.path);
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

    private void UpdateGroundedState()
    {
        if (playerController != null)
        {
            isGrounded = playerController.IsGrounded;
        }
        else if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else if (rb != null)
        {
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }
    }

    private void ApplyMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void UpdateDirtParticles()
    {
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
    }

    private void SpawnDirtFromPool(float directionX)
    {
        if (dirtPrefab == null || dirtPool == null || dirtPool.Length == 0)
        {
            return;
        }

        GameObject dirt = dirtPool[currentPoolIndex];
        DirtParticle particleScript = dirt != null ? dirt.GetComponent<DirtParticle>() : null;
        if (particleScript != null && groundCheck != null)
        {
            particleScript.Spawn(groundCheck.position, directionX);
        }

        currentPoolIndex++;
        if (currentPoolIndex >= dirtPool.Length)
        {
            currentPoolIndex = 0;
        }
    }

    private void UpdateFacingDirection()
    {
        if (MoveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (MoveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void UpdateAnimator()
    {
        if (anim == null)
        {
            return;
        }

        anim.SetBool("isRunning", Mathf.Abs(MoveInput.x) > 0.1f);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void ApplyIdleAnimatorState()
    {
        if (anim == null)
        {
            return;
        }

        anim.SetBool("isRunning", false);
        anim.SetBool("isGrounded", isGrounded);
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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
