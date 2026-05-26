using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public enum ElementType { Fire, Water } // Định nghĩa 2 hệ

    [Header("Cấu hình Hệ của con Ếch này")]
    public ElementType playerElement; 

    [Header("Hình ảnh Chìa khóa trên đầu")]
    [SerializeField] private GameObject keyIndicator; // Cái Sprite chìa khóa nhỏ lơ lửng trên đầu con ếch

    // Biến lưu trữ số lượng ngọc và trạng thái giữ chìa khóa
    public int gemCount { get; private set; } = 0;
    public bool hasKey { get; private set; } = false;

    private void Start()
    {
        // Lúc đầu game chưa nhặt khóa thì ẩn cái hình chìa khóa trên đầu đi
        if (keyIndicator != null)
        {
            keyIndicator.SetActive(false);
        }
    }

    // Hàm xử lý khi nhặt Ngọc (Gem)
    public void CollectGem()
    {
        gemCount++;
        Debug.Log(gameObject.name + " đã nhặt được Ngọc! Tổng số ngọc: " + gemCount);
        // Sau này Mạnh có thể gọi hàm cập nhật giao diện UI số lượng ngọc ở đây
    }

    // Hàm xử lý khi nhặt đúng Chìa khóa
    public void CollectKey()
    {
        hasKey = true;
        
        // Hiện cái chìa khóa lơ lửng trên đầu lên!
        if (keyIndicator != null)
        {
            keyIndicator.SetActive(true);
        }
        
        Debug.Log(gameObject.name + " đã giữ chìa khóa hệ " + playerElement);
    }
    // ========================================================
    // ĐOẠN CODE KẾT NỐI VỚI CHECKPOINT MANAGER BẠN VỪA GỬI ĐÂY NÈ
    // ========================================================

    private Vector3 defaultSpawnPosition;

    // Hàm Start cũ của bạn, Mạnh thêm dòng lưu vị trí gốc vào nha:
    private void Awake()
    {
        // Lưu lại vị trí xuất phát ban đầu phòng trường hợp chưa ăn checkpoint nào
        defaultSpawnPosition = transform.position;
    }

    // Hàm này sẽ được gọi khi con ếch đụng trúng bẫy rìu/răng cưa
    public void RespawnAtCheckpoint()
    {
        if (CheckpointManager.Instance != null)
        {
            // Hỏi CheckpointManager xem vị trí hòn đá mới nhất ở đâu để bay về
            transform.position = CheckpointManager.Instance.GetRespawnPosition(defaultSpawnPosition);
        }
        else
        {
            // Nếu có lỗi gì đó chưa có manager thì bay về vị trí bắt đầu màn chơi
            transform.position = defaultSpawnPosition;
        }

        // Khóa lực quán tính (để ếch không bị rơi tiếp tục sau khi hồi sinh)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log(gameObject.name + " đã hồi sinh tại Checkpoint mới nhất!");
    }
}