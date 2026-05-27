using UnityEngine;

public class ShieldItem : MonoBehaviour
{
    [Header("Lớp khiên bao bọc nhân vật")]
    public GameObject shieldBarrierPrefab; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem ai chạm vào (Đảm bảo 2 con ếch của bạn có Tag là "Player")
        if (collision.CompareTag("Player"))
        {
            if (shieldBarrierPrefab != null)
            {
                // Tạo ra Lớp Khiên Bảo Vệ ngay tại vị trí người chơi
                GameObject barrier = Instantiate(shieldBarrierPrefab, collision.transform.position, Quaternion.identity);
                
                // Lệnh này cực kỳ quan trọng: Ép cái khiên phải "nhận cha", 
                // từ đó nó sẽ dính chặt và di chuyển theo nhân vật nhặt được nó!
                barrier.transform.SetParent(collision.transform); 
            }

            // Tiêu diệt vật phẩm rơi trên đất
            Destroy(gameObject);
        }
    }
}