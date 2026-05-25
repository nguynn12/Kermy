using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Cài đặt Đạn")]
    public float speed = 10f;
    public float lifetime = 3f;
    public float damage = 10f; // Sát thương cơ bản

    [Header("Hệ Nguyên Tố")]
    // Kết nối với danh sách hệ nguyên tố từ script của Boss
    public MiniBossHealth.Element bulletElement; 

    [Header("Phân biệt Đạn (Hệ thống)")]
    // THÊM BIẾN NÀY ĐỂ TRỊ LỖI ĐẠN "TỰ HỦY" VÀ BOSS "TỰ SÁT"
    // Nếu ĐÃ TICK vào ô này -> Đạn của QUÁI. Nếu CHƯA TICK -> Đạn của NGƯỜI CHƠI.
    public bool isEnemyBullet = true; 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- LOGIC MỚI: PHÂN ĐỊNH RACH RÒI BẠN - THÙ ---

        // TRƯỜNG HỢP A: Đạn của QUÁI BẮN RA (isEnemyBullet = TRUE)
        if (isEnemyBullet)
        {
            // Đạn quái CHỈ TẤN CÔNG NGƯỜI CHƠI, BỎ QUA TẤT CẢ QUÁI KHÁC (bao gồm cả Dơi và Boss)
            Level05_PlayerHealth playerHealth = collision.GetComponent<Level05_PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // Ếch mất máu/reset
                Destroy(gameObject); // Viên đạn nổ tung khi trúng người chơi
                return;
            }
            
            // Nếu chạm vào Tag 'Enemy' (như Boss, Dơi...), nó sẽ bỏ qua và bay tiếp, 
            // không tự hủy và không gây sát thương lên quái đồng minh.
            if (collision.CompareTag("Enemy"))
            {
                return; // Bỏ qua va chạm, không làm gì cả
            }
        }
        // TRƯỜNG HỢP B: Đạn của NGƯỜI CHƠI BẮN RA (isEnemyBullet = FALSE)
        else
        {
            // 1. TẤN CÔNG MINI-BOSS (Vật thể có script MiniBossHealth)
            MiniBossHealth bossHealth = collision.GetComponent<MiniBossHealth>();
            if (bossHealth != null)
            {
                // Truyền lượng sát thương và hệ của viên đạn sang cho Boss tính toán
                bossHealth.TakeDamage(damage, bulletElement);
                Destroy(gameObject); // Viên đạn nổ tung khi trúng Boss
                return;
            }

            // 2. TẤN CÔNG QUÁI THƯỜNG (Dơi có Tag Enemy nhưng không có máu)
            if (collision.CompareTag("Enemy"))
            {
                // Chỉ hủy dơi nếu dơi không phải là boss (tránh hủy boss nhầm)
                if (collision.GetComponent<EnemyMovement>() != null)
                {
                    Destroy(collision.gameObject); // Dơi bốc hơi
                    Destroy(gameObject); // Viên đạn nổ tung
                    return;
                }
            }
        }
    }
}