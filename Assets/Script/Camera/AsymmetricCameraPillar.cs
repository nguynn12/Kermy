using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AsymmetricCameraPillar : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CameraController cameraController;

    [Header("Commander")]
    [SerializeField] private PlayerController allowedCommander;

    [Header("Visual State")]
    [SerializeField] private SpriteRenderer stateRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite inUseSprite;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color inUseColor = Color.cyan;

    private PlayerController _candidateCommander;
    private PlayerInputHandler _candidateInput;

    private PlayerController _activeCommander;
    private PlayerInputHandler _activeCommanderInput;
    private bool _isInUse;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (stateRenderer == null)
        {
            stateRenderer = GetComponent<SpriteRenderer>();
        }

        ApplyVisualState(false);
    }

    private void Update()
    {
        if (cameraController == null)
        {
            return;
        }

        if (_isInUse)
        {
            if (_activeCommander == null)
            {
                return;
            }

            if (IsActionPressed(_activeCommander, _activeCommanderInput))
            {
                StopUsing();
            }

            return;
        }

        if (_candidateInput == null)
        {
            return;
        }

        if (_candidateInput.ConsumeActionPressed())
        {
            StartUsing(_candidateCommander, _candidateInput);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null)
        {
            return;
        }

        if (allowedCommander != null && pc != allowedCommander)
        {
            return;
        }

        if (_isInUse)
        {
            return;
        }

        _candidateCommander = pc;
        _candidateInput = pc.GetComponent<PlayerInputHandler>();
        if (_candidateInput != null)
        {
            _candidateInput.ClearActionPressed();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null)
        {
            return;
        }

        if (_candidateCommander == pc)
        {
            _candidateCommander = null;
            _candidateInput = null;
        }
    }

    private void StartUsing(PlayerController commander, PlayerInputHandler commanderInput)
    {
        if (commander == null || commanderInput == null || cameraController == null)
        {
            return;
        }

        _activeCommander = commander;
        _activeCommanderInput = commanderInput;
        _isInUse = true;
        commander.SetControlEnabled(false);
        commander.SetCameraUseAnchored(true);
        ApplyVisualState(true);
        cameraController.EnterAsymmetricSplitMode(this, commander.transform, commanderInput);
    }

    private void StopUsing()
    {
        if (!_isInUse)
        {
            return;
        }

        _isInUse = false;
        cameraController.ExitAsymmetricSplitMode(this);
        if (_activeCommander != null)
        {
            _activeCommander.SetCameraUseAnchored(false);
            _activeCommander.SetControlEnabled(true);
        }

        ApplyVisualState(false);
        _activeCommander = null;
        _activeCommanderInput = null;
    }

    private bool IsActionPressed(PlayerController commander, PlayerInputHandler commanderInput)
    {
        if (commander != null && KeybindingManager.GetActionDownForTag(commander.tag))
        {
            return true;
        }

        return commanderInput != null && commanderInput.ConsumeActionPressed();
    }

    private void ApplyVisualState(bool inUse)
    {
        if (stateRenderer == null)
        {
            return;
        }

        Sprite nextSprite = inUse ? inUseSprite : idleSprite;
        if (nextSprite != null)
        {
            stateRenderer.sprite = nextSprite;
        }

        stateRenderer.color = inUse ? inUseColor : idleColor;
    }

    private void OnDisable()
    {
        StopUsing();
    }
}
