using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundAutoScaler : MonoBehaviour
{
    private Camera _parentCamera;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        // Tự động tìm Camera mẹ mà bức ảnh đang được gắn vào
        _parentCamera = GetComponentInParent<Camera>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (_parentCamera == null || _spriteRenderer.sprite == null) return;

        // 1. Tính toán chiều cao và chiều rộng thực tế của Camera hiện tại
        float cameraHeight = _parentCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _parentCamera.aspect;

        // 2. Lấy kích thước gốc của bức ảnh (lúc chưa Scale)
        Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;

        // 3. Tính toán tỷ lệ cần phóng to
        float scaleX = cameraWidth / spriteSize.x;
        float scaleY = cameraHeight / spriteSize.y;

        // Dùng Mathf.Max để đảm bảo ảnh luôn phủ kín màn hình mà KHÔNG bị méo hình (trăng không bị móp)
        float finalScale = Mathf.Max(scaleX, scaleY);

        // 4. Áp dụng Scale mới cho bức ảnh
        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}