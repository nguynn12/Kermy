using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private enum CameraState
    {
        FollowCentroid = 0,
        StationFreeLook = 1,
        AsymmetricSplit = 2
    }

    [Header("Targets")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Follow (Centroid)")]
    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private Vector2 followOffset;

    // --- ZOOM SETTINGS ĐÃ ĐƯỢC CHUẨN HÓA ---
    [Header("Zoom Settings (Pico Park Style)")]
    [SerializeField] private float minZoom = 6.5f;
    [SerializeField] private float maxZoom = 30f;
    [SerializeField] private float framingPadding = 3f;
    [SerializeField] private float zoomSmoothTime = 5f;
    // ----------------------------------------

    [Header("Camera Bounds")]
    [SerializeField] private bool useCameraBounds;
    [SerializeField] private Transform cameraBoundsMinTransform;
    [SerializeField] private Transform cameraBoundsMaxTransform;
    [SerializeField] private Vector2 cameraBoundsMin;
    [SerializeField] private Vector2 cameraBoundsMax;

    [Header("Station Free-look")]
    [SerializeField] private InputActionReference commanderLookAction;
    [SerializeField] private float freeLookSpeed = 8f;
    [SerializeField] private Vector2 freeLookOffset;

    [Header("Asymmetric Split")]
    [SerializeField, Range(0.1f, 0.9f)] private float leftViewportWidth = 0.3f;
    [SerializeField] private Camera leftCamera;
    [SerializeField] private float leftCameraOrthoSize = 3.5f;
    [SerializeField] private Vector2 leftFollowOffset;
    [SerializeField] private float leftFollowSmoothTime = 0.12f;
    [SerializeField] private int dividerWidthPixels = 6;

    private CameraState _state = CameraState.FollowCentroid;
    private Vector3 _followVelocity;
    private float _zoomVelocity;

    private Camera _rightCamera;
    private Rect _rightCameraDefaultRect;
    private float _rightCameraDefaultOrthoSize;

    private CameraStation _activeStation;
    private bool _lookEnabled;

    private AsymmetricCameraPillar _activePillar;
    private Transform _splitCommander;
    private PlayerInputHandler _splitCommanderInput;
    private Vector3 _splitRightCameraBasePos;
    private Vector3 _leftFollowVelocity;
    private RectTransform _dividerRect;
    private Canvas _dividerCanvas;

    // THÊM HÀM START NÀY ĐỂ TRỊ BỆNH "ĐỨNG IM LÚC ĐẦU"
    private void Start()
    {
        // Ép buộc Camera khởi động ở trạng thái đi theo 2 người
        _state = CameraState.FollowCentroid;

        if (leftCamera != null)
        {
            leftCamera.enabled = false;
        }
    }

    private void OnEnable()
    {
        _rightCamera = GetComponent<Camera>();
        if (_rightCamera != null)
        {
            _rightCameraDefaultRect = _rightCamera.rect;
            _rightCameraDefaultOrthoSize = _rightCamera.orthographicSize;
        }

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
            case CameraState.AsymmetricSplit:
                TickAsymmetricSplit();
                break;
        }
    }

    private void TickFollowCentroid()
    {
        EnsureTargetsAssigned();
        if (player1 == null || player2 == null) return;

        // 1. DI CHUYỂN
        Vector3 centroid = (player1.position + player2.position) * 0.5f;
        Vector3 target = new Vector3(centroid.x + followOffset.x, centroid.y + followOffset.y, transform.position.z);
        // 2. ZOOM CHI KHI CAN THEM KHUNG HINH DE CHUA CA HAI NHAN VAT
        if (_rightCamera != null)
        {
            Vector2 playerDelta = player1.position - player2.position;
            float aspect = Mathf.Max(_rightCamera.aspect, 0.01f);
            float requiredZoomByWidth = (Mathf.Abs(playerDelta.x) * 0.5f + framingPadding) / aspect;
            float requiredZoomByHeight = Mathf.Abs(playerDelta.y) * 0.5f + framingPadding;
            float targetZoom = Mathf.Max(minZoom, requiredZoomByWidth, requiredZoomByHeight);
            
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            _rightCamera.orthographicSize = Mathf.SmoothDamp(
                _rightCamera.orthographicSize,
                targetZoom,
                ref _zoomVelocity,
                Mathf.Max(0.01f, 1f / zoomSmoothTime));
        }

        target = ClampCameraPosition(target);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _followVelocity, followSmoothTime);
        transform.position = ClampCameraPosition(transform.position);
    }

    private void TickStationFreeLook()
    {
        if (!_lookEnabled || commanderLookAction == null || commanderLookAction.action == null) return;

        Vector2 input = commanderLookAction.action.ReadValue<Vector2>();
        Vector3 delta = new Vector3(input.x, input.y, 0f) * (freeLookSpeed * Time.deltaTime);
        transform.position = ClampCameraPosition(transform.position + delta);
    }

    public void FramePlayersImmediately()
    {
        EnsureTargetsAssigned();
        if (player1 == null || player2 == null)
        {
            return;
        }

        if (_rightCamera == null)
        {
            _rightCamera = GetComponent<Camera>();
        }

        Vector3 centroid = (player1.position + player2.position) * 0.5f;
        transform.position = ClampCameraPosition(new Vector3(
            centroid.x + followOffset.x,
            centroid.y + followOffset.y,
            transform.position.z));

        if (_rightCamera == null)
        {
            return;
        }

        Vector2 playerDelta = player1.position - player2.position;
        float aspect = Mathf.Max(_rightCamera.aspect, 0.01f);
        float requiredZoomByWidth = (Mathf.Abs(playerDelta.x) * 0.5f + framingPadding) / aspect;
        float requiredZoomByHeight = Mathf.Abs(playerDelta.y) * 0.5f + framingPadding;
        _rightCamera.orthographicSize = Mathf.Clamp(
            Mathf.Max(minZoom, requiredZoomByWidth, requiredZoomByHeight),
            minZoom,
            maxZoom);
    }

    private void TickAsymmetricSplit()
    {
        if (_rightCamera == null) return;

        if (_splitCommanderInput != null)
        {
            Vector2 input = _splitCommanderInput.MoveInput;
            Vector3 delta = new Vector3(input.x, input.y, 0f) * (freeLookSpeed * Time.deltaTime);
            transform.position = ClampCameraPosition(transform.position + delta);
        }

        if (leftCamera != null && _splitCommander != null)
        {
            Vector3 desired = new Vector3(
                _splitCommander.position.x + leftFollowOffset.x,
                _splitCommander.position.y + leftFollowOffset.y,
                leftCamera.transform.position.z);

            leftCamera.transform.position = ClampCameraPosition(Vector3.SmoothDamp(
                leftCamera.transform.position,
                desired,
                ref _leftFollowVelocity,
                leftFollowSmoothTime));
        }
    }

    public void EnterStationMode(CameraStation station)
    {
        _activeStation = station;
        _state = CameraState.StationFreeLook;
    }

    public void ExitStationMode(CameraStation station)
    {
        if (_activeStation != station) return;
        _activeStation = null;
        _state = CameraState.FollowCentroid;
    }

    public void EnterAsymmetricSplitMode(AsymmetricCameraPillar pillar, Transform commanderTransform, PlayerInputHandler commanderInput)
    {
        _activePillar = pillar;
        _splitCommander = commanderTransform;
        _splitCommanderInput = commanderInput;

        if (_rightCamera == null)
        {
            _rightCamera = GetComponent<Camera>();
            if (_rightCamera != null)
            {
                _rightCameraDefaultRect = _rightCamera.rect;
                _rightCameraDefaultOrthoSize = _rightCamera.orthographicSize;
            }
        }

        if (_rightCamera != null) _rightCamera.rect = new Rect(leftViewportWidth, 0f, 1f - leftViewportWidth, 1f);

        if (leftCamera != null)
        {
            leftCamera.enabled = true;
            leftCamera.rect = new Rect(0f, 0f, leftViewportWidth, 1f);
            leftCamera.orthographicSize = leftCameraOrthoSize;
        }

        EnsureDivider();
        UpdateDividerRect();
        _splitRightCameraBasePos = transform.position;
        _state = CameraState.AsymmetricSplit;
    }

    public void ExitAsymmetricSplitMode(AsymmetricCameraPillar pillar)
    {
        if (_activePillar != pillar) return;

        _activePillar = null;
        _splitCommander = null;
        _splitCommanderInput = null;
        _leftFollowVelocity = Vector3.zero;

        if (_rightCamera != null)
        {
            _rightCamera.rect = _rightCameraDefaultRect;
            _rightCamera.orthographicSize = _rightCameraDefaultOrthoSize;
        }

        if (leftCamera != null) leftCamera.enabled = false;

        if (_dividerRect != null) _dividerRect.gameObject.SetActive(false);

        _state = CameraState.FollowCentroid;
    }

    private void EnsureDivider()
    {
        if (_dividerRect != null)
        {
            _dividerRect.gameObject.SetActive(true);
            return;
        }

        GameObject canvasGo = new GameObject("CameraSplitCanvas");
        _dividerCanvas = canvasGo.AddComponent<Canvas>();
        _dividerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject dividerGo = new GameObject("Divider");
        dividerGo.transform.SetParent(canvasGo.transform, false);

        Image img = dividerGo.AddComponent<Image>();
        img.color = Color.black;

        _dividerRect = dividerGo.GetComponent<RectTransform>();
        _dividerRect.anchorMin = new Vector2(leftViewportWidth, 0f);
        _dividerRect.anchorMax = new Vector2(leftViewportWidth, 1f);
        _dividerRect.pivot = new Vector2(0.5f, 0.5f);
        _dividerRect.anchoredPosition = Vector2.zero;
        _dividerRect.sizeDelta = new Vector2(dividerWidthPixels, 0f);
    }

    private void UpdateDividerRect()
    {
        if (_dividerRect == null) return;

        _dividerRect.gameObject.SetActive(true);
        _dividerRect.anchorMin = new Vector2(leftViewportWidth, 0f);
        _dividerRect.anchorMax = new Vector2(leftViewportWidth, 1f);
        _dividerRect.sizeDelta = new Vector2(dividerWidthPixels, 0f);
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        if (!useCameraBounds || _rightCamera == null) return position;

        float halfHeight = _rightCamera.orthographicSize;
        float halfWidth = halfHeight * Mathf.Max(_rightCamera.aspect, 0.01f);

        Vector2 boundsMin = GetCameraBoundsMin();
        Vector2 boundsMax = GetCameraBoundsMax();

        float minX = boundsMin.x + halfWidth;
        float maxX = boundsMax.x - halfWidth;
        float minY = boundsMin.y + halfHeight;
        float maxY = boundsMax.y - halfHeight;

        if (minX <= maxX)
        {
            position.x = Mathf.Clamp(position.x, minX, maxX);
        }
        else
        {
            position.x = (boundsMin.x + boundsMax.x) * 0.5f;
        }

        if (minY <= maxY)
        {
            position.y = Mathf.Clamp(position.y, minY, maxY);
        }
        else
        {
            position.y = (boundsMin.y + boundsMax.y) * 0.5f;
        }

        return position;
    }

    private Vector2 GetCameraBoundsMin()
    {
        if (cameraBoundsMinTransform != null)
        {
            return cameraBoundsMinTransform.position;
        }

        return cameraBoundsMin;
    }

    private Vector2 GetCameraBoundsMax()
    {
        if (cameraBoundsMaxTransform != null)
        {
            return cameraBoundsMaxTransform.position;
        }

        return cameraBoundsMax;
    }

    private void EnsureTargetsAssigned()
    {
        if (player1 != null && player2 != null)
        {
            return;
        }

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            string normalizedTag = player.tag.Replace(" ", string.Empty);
            if (player1 == null && normalizedTag == "Player1")
            {
                player1 = player.transform;
            }
            else if (player2 == null && normalizedTag == "Player2")
            {
                player2 = player.transform;
            }
        }

        if ((player1 == null || player2 == null) && players.Length >= 2)
        {
            player1 ??= players[0].transform;
            player2 ??= players[1].transform;
        }
    }
}
