using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private enum CameraState
    {
        FollowCentroid = 0,
        StationFreeLook = 1
    }

    [Header("Targets")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Follow (Centroid)")]
    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private Vector2 followOffset;

    [Header("Station Free-look")]
    [SerializeField] private InputActionReference commanderLookAction;
    [SerializeField] private float freeLookSpeed = 8f;
    [SerializeField] private Vector2 freeLookOffset;

    private CameraState _state = CameraState.FollowCentroid;
    private Vector3 _followVelocity;

    private CameraStation _activeStation;
    private bool _lookEnabled;

    private void OnEnable()
    {
        if (commanderLookAction != null && commanderLookAction.action != null)
        {
            commanderLookAction.action.Enable();
            _lookEnabled = true;
        }
    }

    private void OnDisable()
    {
        if (commanderLookAction != null && commanderLookAction.action != null)
        {
            commanderLookAction.action.Disable();
            _lookEnabled = false;
        }
    }

    private void LateUpdate()
    {
        switch (_state)
        {
            case CameraState.FollowCentroid:
                TickFollowCentroid();
                break;
            case CameraState.StationFreeLook:
                TickStationFreeLook();
                break;
        }
    }

    private void TickFollowCentroid()
    {
        if (player1 == null || player2 == null)
        {
            return;
        }

        Vector3 centroid = (player1.position + player2.position) * 0.5f;
        Vector3 target = new Vector3(centroid.x + followOffset.x, centroid.y + followOffset.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _followVelocity, followSmoothTime);
    }

    private void TickStationFreeLook()
    {
        if (!_lookEnabled || commanderLookAction == null || commanderLookAction.action == null)
        {
            return;
        }

        Vector2 input = commanderLookAction.action.ReadValue<Vector2>();
        Vector3 delta = new Vector3(input.x, input.y, 0f) * (freeLookSpeed * Time.deltaTime);
        transform.position += delta; 
    }

    public void EnterStationMode(CameraStation station)
    {
        _activeStation = station;
        _state = CameraState.StationFreeLook;
    }

    public void ExitStationMode(CameraStation station)
    {
        if (_activeStation != station)
        {
            return;
        }

        _activeStation = null;
        _state = CameraState.FollowCentroid;
    }
}
