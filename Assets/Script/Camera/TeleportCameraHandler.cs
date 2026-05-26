using System.Collections;
using UnityEngine;

// Gắn vào Main Camera (cùng chỗ CameraController)
// Script này theo dõi khoảng cách 2 nhân vật
// Khi teleport xa nhau → split screen
// Khi lại gần → gộp lại
public class TeleportCameraHandler : MonoBehaviour
{
    [Header("Kéo 2 nhân vật vào đây")]
    [SerializeField] private Transform player1;  // Ignus
    [SerializeField] private Transform player2;  // Aqua

    [Header("Kéo CameraController vào đây")]
    [SerializeField] private CameraController cameraController;

    [Header("Kéo Left Camera vào đây (LeftSplitCamera)")]
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera rightCamera; // Main Camera

    [Header("Khoảng cách bao nhiêu thì split? (đơn vị Unity)")]
    [SerializeField] private float splitThreshold = 12f;
    // Khi 2 người cách nhau hơn 12 unit → split
    // Khi teleport, khoảng cách thường > 12 ngay lập tức

    [Header("Tốc độ camera bám theo nhân vật khi split")]
    [SerializeField] private float splitFollowSpeed = 5f;

    [Header("Màu đường kẻ giữa màn hình")]
    [SerializeField] private Color dividerColor = Color.black;

    // ── Biến nội bộ ───────────────────────────────────────────
    private bool _isSplit = false;
    // true = đang split screen, false = đang chạy bình thường

    private Vector3 _cam1Velocity;
    private Vector3 _cam2Velocity;
    // Dùng cho SmoothDamp — tốc độ hiện tại của camera

    private UnityEngine.UI.Image _divider;
    // Đường kẻ đen giữa màn hình

    // ── Awake ─────────────────────────────────────────────────
    private void Awake()
    {
        // Tạo đường kẻ giữa ngay từ đầu nhưng ẩn đi
        CreateDivider();
        SetSplitActive(false);
    }

    // ── Update: kiểm tra mỗi frame ────────────────────────────
    private void Update()
    {
        if (player1 == null || player2 == null) return;

        float distance = Vector2.Distance(player1.position, player2.position);
        // Vector2.Distance = khoảng cách giữa 2 điểm trên mặt phẳng 2D

        bool shouldSplit = distance > splitThreshold;
        // true nếu khoảng cách vượt ngưỡng

        if (shouldSplit && !_isSplit)
        {
            // Vừa vượt ngưỡng → bắt đầu split
            StartSplit();
        }
        else if (!shouldSplit && _isSplit)
        {
            // Đã lại gần nhau → gộp lại
            EndSplit();
        }

        // Nếu đang split → cập nhật vị trí 2 camera
        if (_isSplit)
        {
            FollowPlayersInSplit();
        }
    }

    // ── Bắt đầu split screen ──────────────────────────────────
    private void StartSplit()
    {
        _isSplit = true;
        SetSplitActive(true);

        // Tắt CameraController để nó không tranh quyền điều khiển camera
        if (cameraController != null)
            cameraController.enabled = false;
        // Khi disabled, CameraController không chạy LateUpdate nữa
        // → Script này toàn quyền điều khiển camera

        // Chia viewport:
        // Right camera (Main Camera) → nửa phải màn hình
        if (rightCamera != null)
            rightCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        // Rect(x, y, width, height)
        // x=0.5 nghĩa là bắt đầu từ giữa màn hình
        // width=0.5 nghĩa là chiếm nửa phải

        // Left camera → nửa trái màn hình
        if (leftCamera != null)
        {
            leftCamera.enabled = true;
            leftCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
            // x=0 bắt đầu từ trái, width=0.5 chiếm nửa trái
        }
    }

    // ── Kết thúc split screen ─────────────────────────────────
    private void EndSplit()
    {
        _isSplit = false;
        SetSplitActive(false);

        // Bật lại CameraController
        if (cameraController != null)
            cameraController.enabled = true;

        // Trả viewport về toàn màn hình
        if (rightCamera != null)
            rightCamera.rect = new Rect(0f, 0f, 1f, 1f);

        if (leftCamera != null)
            leftCamera.enabled = false;
    }

    // ── Camera bám theo từng nhân vật khi đang split ──────────
    private void FollowPlayersInSplit()
    {
        // Right camera bám Ignus (player1)
        if (rightCamera != null && player1 != null)
        {
            Vector3 target1 = new Vector3(
                player1.position.x,
                player1.position.y,
                rightCamera.transform.position.z
            // Giữ nguyên Z — camera 2D không thay đổi Z
            );
            rightCamera.transform.position = Vector3.SmoothDamp(
                rightCamera.transform.position,
                target1,
                ref _cam1Velocity,
                1f / splitFollowSpeed
            // SmoothDamp nhận smoothTime, không phải speed
            // 1/speed để chuyển đổi
            );
        }

        // Left camera bám Aqua (player2)
        if (leftCamera != null && player2 != null)
        {
            Vector3 target2 = new Vector3(
                player2.position.x,
                player2.position.y,
                leftCamera.transform.position.z
            );
            leftCamera.transform.position = Vector3.SmoothDamp(
                leftCamera.transform.position,
                target2,
                ref _cam2Velocity,
                1f / splitFollowSpeed
            );
        }
    }

    // ── Tạo đường kẻ giữa màn hình ───────────────────────────
    private void CreateDivider()
    {
        // Tạo Canvas
        var canvasGo = new GameObject("TeleportSplitDivider");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // ScreenSpaceOverlay = vẽ lên trên cùng, không bị che

        canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();

        // Tạo đường kẻ đen
        var dividerGo = new GameObject("Line");
        dividerGo.transform.SetParent(canvasGo.transform, false);

        _divider = dividerGo.AddComponent<UnityEngine.UI.Image>();
        _divider.color = dividerColor;

        var rect = dividerGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        // Anchor ở giữa màn hình, kéo dài từ trên xuống dưới

        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(4f, 0f);
        // Đường kẻ rộng 4px
    }

    // ── Bật/tắt đường kẻ và trạng thái split ─────────────────
    private void SetSplitActive(bool active)
    {
        if (_divider != null)
            _divider.gameObject.SetActive(active);
    }

    // ── Gọi từ TeleportPortal khi teleport xảy ra ─────────────
    // Dùng để force split ngay lập tức thay vì chờ threshold
    public void OnPlayerTeleported()
    {
        if (!_isSplit)
            StartSplit();
        // Gọi cái này từ TeleportPortal.cs sau khi teleport xong
    }
}