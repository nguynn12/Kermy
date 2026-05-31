using UnityEngine;

public class BossUltimate : MonoBehaviour
{
    [Header("Cài đặt đạn Ulti")]
    public float speed = 10f; 
    public float lifeTime = 6f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- ĐOẠN MỚI THÊM VÀO: KIỂM TRA VA CHẠM VỚI KHIÊN ---
        if (collision.CompareTag("Shield")) 
        {
            // Nếu đập trúng khiên, quả cầu sẽ nổ tung (bị xóa) ngay lập tức
            Destroy(gameObject);
            return; // Lệnh này giúp code thoát ra ngay, không chạy xuống phần trừ máu bên dưới nữa
        }

        // --- PHẦN TRỪ MÁU NGƯỜI CHƠI (Giữ nguyên) ---
        if (collision.CompareTag("Player"))
        {
            PlayerCombatHealth frogHealth = collision.GetComponent<PlayerCombatHealth>();
            
            if (frogHealth != null)
            {
                frogHealth.TakeDamage(); 
                frogHealth.TakeDamage(); 
            }
            
            Destroy(gameObject);
        }
    }
}