using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
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
        // Backward compatibility: if you don't have a dedicated jump action yet,
        // allow "jump" to come from MoveInput.y like the old setup.
        if (_inputEnabled && jumpAction == null && moveAction != null)
        {
            if (MoveInput.y > 0.5f)
            {
                _jumpPressedThisFrame = true;
            }
        }
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
            // Clear state so you don't get a buffered jump/action when re-enabling.
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
        // For local co-op with multiple action maps, you typically enable actions on the PlayerInput,
        // but this per-component toggle is useful for camera-station control locks.
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
        if (!_inputEnabled)
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled)
        {
            return;
        }

        // Use "performed" as the edge trigger.
        if (ctx.performed)
        {
            _jumpPressedThisFrame = true;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled)
        {
            IsActionHeld = false;
            return;
        }

        // For buttons, started/performed/canceled can vary by interaction type.
        // This keeps both "held" and "pressed this frame" clean.
        if (ctx.started)
        {
            IsActionHeld = true;
            _actionPressedThisFrame = true;
        }
        else if (ctx.canceled)
        {
            IsActionHeld = false;
        }
    }

    private static void BindAndEnable(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return;
        }

        actionRef.action.performed += callback;
        actionRef.action.canceled += callback;
        actionRef.action.started += callback;

        actionRef.action.Enable();
    }

    private static void UnbindAndDisable(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return;
        }

        actionRef.action.performed -= callback;
        actionRef.action.canceled -= callback;
        actionRef.action.started -= callback;

        actionRef.action.Disable();
    }

    private static void EnableIfNotNull(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Enable();
        }
    }

    private static void DisableIfNotNull(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Disable();
        }
    }
}