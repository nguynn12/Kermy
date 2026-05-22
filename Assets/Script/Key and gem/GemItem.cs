using UnityEngine;

public class GemItem : MonoBehaviour
{
    // Tạo bảng chọn màu sắc trực quan ngoài Unity Inspector
    public enum GemColorType { Red, Green, Blue }
    
    [Header("Cấu hình viên Ngọc")]
    public GemColorType gemColor; // Chọn màu cho viên ngọc này ngoài Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem vật thể chạm vào có phải là Player không (Dựa vào Tag)
        if (collision.CompareTag("Player 1") || collision.CompareTag("Player 2"))
        {
            // 🌟 ĐOẠN CODE PHÉP THUẬT: Gọi trực tiếp "Bộ não" GemControl đang chạy trên Map
            if (GemControl.Instance != null)
            {
                // Dựa vào màu của viên ngọc này để gọi hàm cộng điểm tương ứng
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