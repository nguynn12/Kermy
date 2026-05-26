using UnityEngine;

public class PatrollingSaw : MonoBehaviour
{
    [Header("Cấu hình di chuyển")]
    [SerializeField] private float speed = 4f;        // Tốc độ di chuyển nhanh/chậm
    [SerializeField] private float moveDistance = 5f; // Khoảng cách chạy sang hai bên từ vị trí gốc

    [Header("Tốc độ tự xoay bánh răng")]
    [SerializeField] private float rotationSpeed = 300f; // Tốc độ xoay tròn của lưỡi cưa (độ/giây)

    private Vector3 startPosition;
    private int direction = 1; // 1 là đi qua phải, -1 là đi qua trái

    private void Start()
    {
        // Lưu lại vị trí ban đầu lúc bạn đặt lưỡi cưa trên Scene làm mốc trung tâm
        startPosition = transform.position;
    }

    private void Update()
    {
        // 1. Tự động xoay tròn bánh răng cưa nhìn cho nguy hiểm
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // 2. Tính toán di chuyển qua lại trên trục X
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

        // Kiểm tra nếu lưỡi cưa vượt quá khoảng cách giới hạn sang bên phải
        if (transform.position.x >= startPosition.x + moveDistance)
        {
            direction = -1; // Đổi hướng sang trái
        }
        // Kiểm tra nếu lưỡi cưa vượt quá khoảng cách giới hạn sang bên trái
        else if (transform.position.x <= startPosition.x - moveDistance)
        {
            direction = 1; // Đổi hướng sang phải
        }
    }

    // Logic va chạm: Chạm vào là chết
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật thể chạm phải có chứa component quản lý máu của Ếch không
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            // Triệt tiêu vận tốc vật lý của Ếch để tránh bị lỗi đơ người khi hồi sinh
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            // Gọi hàm chết để đưa Ếch về Checkpoint gần nhất
            health.Kill();
        }
    }
}