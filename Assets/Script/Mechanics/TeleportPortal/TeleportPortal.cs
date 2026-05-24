using System.Collections;
using UnityEngine;

// Gắn script này vào CẢ 3 cổng
// Mỗi cổng chỉ cần trỏ "destination" sang cổng kia là xong
public class TeleportPortal : MonoBehaviour
{
    [Header("Cổng đích — kéo cổng tiếp theo vào đây")]
    [SerializeField] private Transform destination;
    // Cổng A: kéo Cổng B vào đây
    // Cổng B: kéo Cổng C vào đây
    // Cổng C: kéo Cổng A vào đây

    [Header("Thời gian miễn dịch sau khi teleport (giây)")]
    [SerializeField] private float cooldown = 1f;
    // Tránh bị teleport liên tục khi vừa xuất hiện tại cổng đích

    [Header("Màu cổng (để phân biệt A B C)")]
    [SerializeField] private Color portalColor = Color.magenta;

    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite != null)
            _sprite.color = portalColor;
        // Đổi màu theo cổng — A tím, B xanh, C cam
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ teleport nhân vật, không teleport box hay vật thể khác
        if (other.GetComponent<PlayerController>() == null) return;

        // Kiểm tra nhân vật có đang trong cooldown không
        var status = other.GetComponent<TeleportStatus>();
        if (status != null && status.IsImmune) return;
        // Nếu vừa teleport xong → bỏ qua, không teleport tiếp

        // Thực hiện teleport
        other.transform.position = destination.position;
        // Dịch chuyển nhân vật đến đúng vị trí cổng đích
        // Đơn giản vậy thôi — chỉ 1 dòng!

        // Bắt đầu đếm cooldown trên nhân vật vừa teleport
        if (status == null)
            status = other.gameObject.AddComponent<TeleportStatus>();
        // Nếu chưa có TeleportStatus thì tự thêm vào

        status.StartCooldown(cooldown);

        // Hiệu ứng nhỏ: rung nhẹ camera (tùy chọn)
        StartCoroutine(FlashPortal());
    }

    private IEnumerator FlashPortal()
    {
        // Làm cổng sáng lên 1 chút khi có người đi qua
        if (_sprite == null) yield break;

        Color original = _sprite.color;
        _sprite.color = Color.white;
        // Đổi sang trắng ngay lập tức

        yield return new WaitForSeconds(0.1f);
        // Chờ 0.1 giây
        // IEnumerator + yield return = cách làm animation đơn giản trong Unity
        // Không cần Update, không cần timer biến ngoài

        _sprite.color = original;
        // Trả lại màu gốc
    }
}