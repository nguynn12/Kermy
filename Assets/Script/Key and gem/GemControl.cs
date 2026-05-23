using UnityEngine;

public class GemItem : MonoBehaviour
{
    // Tạo bảng chọn màu sắc trực quan ngoài Unity Inspector
    public enum GemColorType { Red, Green, Blue }
    
    [Header("Cấu hình viên Ngọc")]
    public GemColorType gemColor; // Chọn màu cho viên ngọc này ngoài Inspector

    // ====================================================================
    // 🎵 CHỈ THÊM Ô NÀY ĐỂ KÉO FILE ÂM THANH PICKUP NGOÀI UNITY INSPECTOR
    // ====================================================================
    [Header("Âm thanh Nhặt Ngọc")]
    [SerializeField] private AudioClip gemPickupSound; 

    // ====================================================================
    // 🔒 Ổ KHÓA 1: Giúp ếch chạm vào viên ngọc chỉ tính đúng 1 lần duy nhất
    // ====================================================================
    private bool isCollected = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu viên ngọc này đã được ăn rồi thì chặn lại ngay không cho chạy tiếp
        if (isCollected) return;

        // Kiểm tra xem vật thể chạm vào có phải là Player không (Dựa vào Tag)
        if (collision.CompareTag("Player 1") || collision.CompareTag("Player 2"))
        {
            // 🌟 ĐOẠN CODE PHÉP THUẬT: Gọi trực tiếp "Bộ não" GemControl đang chạy trên Map
            if (GemControl.Instance != null)
            {
                // Sập ổ khóa lại ngay lập tức trước khi cộng điểm
                isCollected = true; 

                // Dựa vào màu của viên ngọc này để gọi hàm cộng điểm tương ứng (GIỮ NGUYÊN CODE CŨ CỦA MẠNH)
                if (gemColor == GemColorType.Red)
                {
                    GemControl.Instance.AddRedGem();
                }
                else if (gemColor == GemColorType.Green)
                {
                    GemControl.Instance.AddGreenGem();
                }
                else if (gemColor == GemColorType.Blue)
                {
                    GemControl.Instance.AddBlueGem();
                }

                // ====================================================================
                // 🎵 PHÁT TIẾNG TING TING NGAY TẠI VỊ TRÍ VIÊN NGỌC (TRƯỚC KHI DESTROY)
                // ====================================================================
                if (gemPickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(gemPickupSound, transform.position, 0.7f);
                }

                // Ăn xong thì cho viên ngọc biến mất khỏi bản đồ
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Mạnh ơi! Chưa có Object nào gắn script GemControl ngoài Hierarchy kìa!");
            }
        }
    }
}