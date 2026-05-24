using UnityEngine;

public class DirtParticle : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector2 velocity;
    
    [Header("Cài đặt Hạt")]
    public float lifeTime = 0.5f; // Sống trong 0.5 giây
    public float gravity = 15f;   // Trọng lực kéo xuống
    
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Hàm này được gọi mỗi khi kích hoạt một hạt đất từ Pool
    public void Spawn(Vector2 spawnPosition, float playerMoveDirection)
    {
        transform.position = spawnPosition;
        timer = lifeTime;
        
        // Reset lại màu sắc (độ trong suốt)
        sr.color = Color.white;

        // --- GỢI Ý NÂNG CAO: RANDOM ĐỂ TRÔNG TỰ NHIÊN ---
        // 1. Random kích thước (Scale)
        float randomScale = Random.Range(0.6f, 1.2f);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // 2. Random góc xoay ban đầu
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // 3. Random vận tốc: Bay NGƯỢC hướng di chuyển của nhân vật
        // Nếu player đi phải (playerMoveDirection > 0), hạt bay sang trái (âm) và ngược lại
        float moveDir = (playerMoveDirection > 0) ? -1f : 1f;
        float speedX = Random.Range(1f, 3f) * moveDir; 
        float speedY = Random.Range(2f, 5f); // Lực nảy lên trên
        
        velocity = new Vector2(speedX, speedY);

        // Bật hiển thị
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Trừ thời gian sống
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false); // Trả về Pool khi hết tuổi thọ
            return;
        }

        // --- Áp dụng Vật lý (Không cần Rigidbody) ---
        velocity.y -= gravity * Time.deltaTime; // Trọng lực kéo Y xuống dần
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // --- Mờ dần (Fade Out) ---
        // Tính tỷ lệ % thời gian còn lại (từ 1 giảm về 0)
        float alpha = timer / lifeTime;
        sr.color = new Color(1, 1, 1, alpha);
    }
}