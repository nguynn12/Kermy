using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 8f; // Tốc độ bay của đạn sang trái
    public float lifeTime = 5f; // Thời gian tự hủy sau khi bắn (tránh rác bộ nhớ)

    void Start()
    {
        // Tự động xóa viên đạn khỏi game sau 5 giây nếu nó bay trượt ra ngoài màn hình
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Di chuyển viên đạn tịnh tiến sang phía bên trái màn hình
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. (Đoạn đạn chạm khiên đã được xử lý ở script ShieldBarrier rồi, bạn không cần viết vào đây)

        // 2. Nếu đạn lọt qua được và đập trúng Người chơi
        if (collision.CompareTag("Player"))
        {
            // Tìm script máu CÓ SẴN của bạn trên con Ếch
            Level05_PlayerHealth frogHealth = collision.GetComponent<Level05_PlayerHealth>();
            
            if (frogHealth != null)
            {
                // Gọi hàm TakeDamage của bạn (hàm của bạn không yêu cầu truyền số vào)
                frogHealth.TakeDamage(); 
            }

            // Hủy viên đạn tím ngay lập tức để không xuyên táo
            Destroy(gameObject);
        }
    }
}