using UnityEngine;

public class CameraStation : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float interactRadius = 1.2f;

    [Header("Commander")]
    [SerializeField] private PlayerController commander;

    private PlayerInputHandler _commanderInput;
    private bool _isInUse;

    private void Awake()
    {
        if (commander != null)
        {
            _commanderInput = commander.GetComponent<PlayerInputHandler>();
        }
    }

    private void Update()
    {
        if (commander == null || _commanderInput == null || cameraController == null)
        {
            return;
        }

        if (!IsCommanderInRange())
        {
            return;
        }

        if (_commanderInput.ConsumeActionPressed())
        {
            ToggleUse();
        }
    }

    private bool IsCommanderInRange()
    {
        if (commander == null)
        {
            return false;
        }
        
        float distance = Vector2.Distance(transform.position, commander.transform.position);
        return distance <= interactRadius;
    }

    private void ToggleUse()
    {
        if (!_isInUse)
        {
            _isInUse = true;
            commander.SetControlEnabled(false);
            cameraController.EnterStationMode(this);
        }
        else
        {
            _isInUse = false;
            cameraController.ExitStationMode(this);
            commander.SetControlEnabled(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
