using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AsymmetricCameraPillar : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CameraController cameraController;

    [Header("Commander")]
    [SerializeField] private PlayerController allowedCommander;

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
    }

    private void Update()
    {
        if (cameraController == null)
        {
            return;
        }

        if (_isInUse)
        {
            if (_activeCommanderInput == null)
            {
                return;
            }

            if (_activeCommanderInput.ConsumeActionPressed())
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
        commander.SetMovementEnabled(false);
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
            _activeCommander.SetMovementEnabled(true);
        }

        _activeCommander = null;
        _activeCommanderInput = null;
    }
}
