using UnityEngine;

public class ElementalHazard : MonoBehaviour
{
    [SerializeField] private ElementalType hazardType;

    public ElementalType HazardType => hazardType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ƯU TIÊN KIỂM TRA NHÂN VẬT TRƯỚC
        ElementalIdentity identity = other.GetComponent<ElementalIdentity>();
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (identity != null && health != null)
        {
            // Kiểm tra: Nếu là bẫy Độc (Toxic) HOẶC nhân vật đi sai hệ nguyên tố (Ếch Nước vào Lava)
            if (hazardType == ElementalType.Toxic || identity.Type != hazardType)
            {
                Debug.Log(other.name + " dính bẫy nguyên tố, xử lý dọn dẹp vật lý để tránh khựng!");

                // 🔊 KÍCH HOẠT TIẾNG CHẾT: Tìm PlayerController trên con ếch chạm bẫy để phát tiếng gầm rú
                PlayerController pControl = other.GetComponent<PlayerController>();
                if (pControl != null)
                {
                    pControl.PlayDeathSound();
                }

                // XỬ LÝ TRIỆT TIÊU VẬT LÝ KHÔNG CHO KHỰNG:
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero; // Đưa vận tốc di chuyển về 0 ngay lập tức
                    rb.angularVelocity = 0f;          // Tắt hoàn toàn vận tốc xoay
                    rb.bodyType = RigidbodyType2D.Kinematic; // Chuyển tạm sang Kinematic để đóng băng vật lý
                }

                // Gọi hàm chết gốc của bạn để dịch chuyển nhân vật về Checkpoint
                health.Kill();

                // TRẢ LẠI TRẠNG THÁI VẬT LÝ SAU 1 NHỊP (Dùng Invoke để tránh bị hút ngược lại hố Lava)
                if (rb != null)
                {
                    // Tạo một hàm nhỏ chạy ẩn sau 0.02 giây để đảm bảo ếch đã "tốc biến" về checkpoint an toàn rồi mới bật lại Dynamic
                    StartCoroutine(ResetPhysicsCoroutine(rb));
                }
                
                return;
            }
            
            // Đúng hệ (Ếch Lửa vào Lava, Ếch Nước vào Nước) -> Đi qua an toàn
            return; 
        }

        // 2. NẾU KHÔNG PHẢI NHÂN VẬT THÌ MỚI XÉT ĐẾN KHỐI HỘP ĐẨY (BLOCK)
        BlockHazardInteraction blockInteraction = other.GetComponent<BlockHazardInteraction>();
        if (blockInteraction != null)
        {
            blockInteraction.OnTouchedHazard(this);
            return;
        }
    }

    // Hàm Coroutine phụ trợ chạy ngầm để trả lại vật lý Dynamic mượt mà không lỗi vị trí
    private System.Collections.IEnumerator ResetPhysicsCoroutine(Rigidbody2D rb)
    {
        yield return new WaitForFixedUpdate(); // Chờ Unity cập nhật xong vị trí mới ở checkpoint
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Trả lại tự do cho ếch di chuyển tiếp
        }
    }
}