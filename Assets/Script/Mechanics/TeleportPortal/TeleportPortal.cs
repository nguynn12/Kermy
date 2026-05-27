using System.Collections;
using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    [Header("Cổng đích — kéo cổng tiếp theo vào đây")]
    [SerializeField] private Transform destination;
    // Cổng A → kéo Cổng B vào
    // Cổng B → kéo Cổng C vào
    // Cổng C → kéo Cổng A vào

    [Header("Thời gian miễn dịch sau khi teleport (giây)")]
    [SerializeField] private float cooldown = 1f;

    // ── Biến nội bộ ───────────────────────────────────────────
    private Animator _animator;
    // Animator để chạy animation có sẵn trên sprite

    private static readonly int FlashTrigger = Animator.StringToHash("Flash");
    // Tên trigger animation khi có người đi qua
    // Nếu Animator không có trigger "Flash" thì không sao — code vẫn chạy bình thường

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // Lấy Animator nếu có — không bắt buộc
        // Nếu bạn setup Animator Controller cho portal thì nó sẽ tự chạy
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ teleport nhân vật
        if (other.GetComponent<PlayerController>() == null) return;

        // Kiểm tra cooldown
        var status = other.GetComponent<TeleportStatus>();
        if (status != null && status.IsImmune) return;

        // ① Teleport nhân vật đến cổng đích
        other.transform.position = destination.position;
        AudioManager.Instance?.PlayTeleport();

        // ② Báo camera frame lại cả hai nhân vật
        var camHandler = FindAnyObjectByType<TeleportCameraHandler>();
        if (camHandler != null)
        {
            camHandler.OnPlayerTeleported();
        }
        else
        {
            var cameraController = FindAnyObjectByType<CameraController>();
            if (cameraController != null)
            {
                cameraController.FramePlayersImmediately();
            }
        }

        // ③ Bắt đầu cooldown
        if (status == null)
            status = other.gameObject.AddComponent<TeleportStatus>();
        status.StartCooldown(cooldown);

        // ④ Kích hoạt hiệu ứng flash trên sprite
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        // Phát trigger animation nếu có Animator
        if (_animator != null)
            _animator.SetTrigger(FlashTrigger);

        // Hiệu ứng scale phồng lên xẹp xuống khi có người đi qua
        // Trông giống cổng "hút" nhân vật vào
        Vector3 originalScale = transform.localScale;
        Vector3 bigScale = originalScale * 1.2f;
        // Phóng to 20%

        float elapsed = 0f;
        float halfDuration = 0.08f;

        // Phóng to
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, bigScale, t);
            yield return null;
            // yield return null = chờ đến frame tiếp theo
        }

        elapsed = 0f;

        // Thu nhỏ về lại
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(bigScale, originalScale, t);
            yield return null;
        }

        // Đảm bảo về đúng scale gốc
        transform.localScale = originalScale;

    }
}
