using UnityEngine;

public class ShieldBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu cái chạm vào là Đạn của Boss (Nhớ kiểm tra Tag của viên đạn tím nhé)
        if (collision.CompareTag("EnemyBullet"))
        {
            // Tiêu diệt viên đạn ngay lập tức để nó không trúng người chơi
            Destroy(collision.gameObject);
            Debug.Log("Khiên đã chặn 1 viên đạn!");
        }
    }
}