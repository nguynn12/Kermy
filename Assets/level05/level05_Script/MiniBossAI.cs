using UnityEngine;

public class MiniBossAI : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float moveSpeed = 2f;
    public float moveDistance = 1.5f; // Tầm bay lên xuống lanh quanh mốc

    [Header("Cài đặt Tấn công")]
    public GameObject dragonBulletPrefab;
    public Transform firePoint;     
    public float fireRate = 3f;     

    [Header("Cơ chế Đổi Vị Trí (Synergy Swap)")]
    public int shotsBeforeSwap = 3;   // Số lần khạc đạn trước khi đổi chỗ
    public float swapSpeed = 8f;      // Tốc độ bay lướt qua nhau (Nên để nhanh cho ngầu)

    private float baseY;              // Vị trí mốc hiện tại (trên hoặc dưới)
    private bool isMovingUp = true;
    private float nextFireTime;
    private Animator anim;
    
    // Các biến kiểm soát trạng thái
    private int currentShotCount = 0;
    private bool isSwapping = false;
    private float targetSwapY;

    void Start()
    {
        // Ghi nhớ vị trí mốc ban đầu khi Spawner gọi ra (vd: 2.5 hoặc -2.5)
        baseY = transform.position.y; 
        anim = GetComponent<Animator>();
        nextFireTime = Time.time + fireRate;
    }

    void Update()
    {
        // Nếu đang trong trạng thái đổi vị trí, ưu tiên chạy lệnh đổi chỗ
        if (isSwapping)
        {
            PerformSwap();
        }
        else
        {
            MoveUpDown();
            HandleShooting();
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
            
            // Tăng biến đếm số lần bắn
            currentShotCount++;
            
            // Đủ 3 lần bắn thì kích hoạt cơ chế đổi chỗ
            if (currentShotCount >= shotsBeforeSwap)
            {
                StartSwapping();
            }
        }
    }

    void Shoot()
    {
        if (anim != null) anim.SetTrigger("Attack");
        
        if (dragonBulletPrefab != null && firePoint != null)
        {
            Instantiate(dragonBulletPrefab, firePoint.position, Quaternion.identity);
        }
    }

    void MoveUpDown()
    {
        // Di chuyển lên xuống mượt mà quanh trục mốc hiện tại (baseY)
        if (isMovingUp)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            if (transform.position.y >= baseY + moveDistance) isMovingUp = false;
        }
        else
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            if (transform.position.y <= baseY - moveDistance) isMovingUp = true;
        }
    }

    void StartSwapping()
    {
        isSwapping = true;
        currentShotCount = 0; // Reset bộ đếm đạn
        
        // Đảo ngược vị trí Y (Ví dụ: Từ mốc trên 2.5 đổi xuống mốc dưới -2.5)
        targetSwapY = -baseY; 
        
        // Tính toán thời gian bay qua nhau để tạm dừng khạc đạn trong lúc bay
        float timeToSwap = Mathf.Abs(targetSwapY - transform.position.y) / swapSpeed;
        nextFireTime = Time.time + timeToSwap + (fireRate / 2f); // Nghỉ một nhịp sau khi đổi chỗ xong mới bắn tiếp
    }

    void PerformSwap()
    {
        // Bay lao thẳng về vị trí đối diện (tạo ra hình chữ X đan qua nhau ở giữa màn hình)
        Vector3 targetPos = new Vector3(transform.position.x, targetSwapY, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, swapSpeed * Time.deltaTime);

        // Chốt vị trí khi đã tới đích
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; 
            baseY = targetSwapY;            // Cập nhật mốc mới
            isSwapping = false;             // Trở lại trạng thái bình thường (bắn + di chuyển)
        }
    }
}