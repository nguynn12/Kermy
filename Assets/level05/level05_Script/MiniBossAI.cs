using UnityEngine;

public class MiniBossAI : MonoBehaviour
{
    [Header("Hiệu ứng Xuất hiện")]
    public float flyInSpeed = 5f; // Tốc độ rồng lao từ ngoài vào
    private Vector3 combatPosition; // Điểm đứng chiến đấu gốc
    private bool hasReachedCombatPosition = false; // Khóa: Chưa bay đến nơi thì chưa được bắn

    [Header("Cài đặt Di chuyển")]
    public float moveSpeed = 2f;
    public float moveDistance = 1.5f; 

    [Header("Cài đặt Tấn công")]
    public GameObject dragonBulletPrefab;
    public Transform firePoint;     
    public float fireRate = 3f;     

    [Header("Cơ chế Đổi Vị Trí (Synergy Swap)")]
    public int shotsBeforeSwap = 3;  
    public float swapSpeed = 8f;      

    [Header("Âm Thanh Rồng")]
    public AudioClip elementShootSound; // Ô chứa file tiếng bắn nguyên tố
    private AudioSource audioSource;    // Biến kết nối tới cái loa

    private float baseY;              
    private bool isMovingUp = true;
    private float nextFireTime;
    private Animator anim;
    
    // Các biến kiểm soát trạng thái
    private int currentShotCount = 0;
    private bool isSwapping = false;
    private float targetSwapY;

    void Start()
    {
        // 1. Tự động kết nối loa
        audioSource = GetComponent<AudioSource>();

        // 2. Ghi nhớ vị trí mốc ban đầu khi Spawner gọi ra (Đây là đích đến của chúng)
        combatPosition = transform.position; 
        baseY = combatPosition.y; 
        
        // 3. Ép rồng văng ra tuốt lề bên phải màn hình (Tọa độ X = 15)
        transform.position = new Vector3(15f, combatPosition.y, combatPosition.z);

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- GIAI ĐOẠN 1: BAY TỪ NGOÀI VÀO (CHÀO SÂN) ---
        if (!hasReachedCombatPosition)
        {
            FlyInRoutine();
        }
        // --- GIAI ĐOẠN 2: CHIẾN ĐẤU (CODE CŨ CỦA BẠN) ---
        else
        {
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
    }

    // Hàm xử lý việc bay từ ngoài vào
    void FlyInRoutine()
    {
        transform.position = Vector3.MoveTowards(transform.position, combatPosition, flyInSpeed * Time.deltaTime);

        // Khi đã cách đích dưới 0.1 unit -> Chốt vị trí và cho phép chiến đấu
        if (Vector3.Distance(transform.position, combatPosition) < 0.1f)
        {
            transform.position = combatPosition; 
            hasReachedCombatPosition = true;
            nextFireTime = Time.time + 1f; // Nghỉ 1 giây ngầu lòi rồi mới bắt đầu khạc đạn
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
            
            currentShotCount++;
            
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

        // KÍCH HOẠT ÂM THANH BẮN NGUYÊN TỐ
        if (elementShootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(elementShootSound); 
        }
    }

    void MoveUpDown()
    {
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
        currentShotCount = 0; 
        
        targetSwapY = -baseY; 
        
        float timeToSwap = Mathf.Abs(targetSwapY - transform.position.y) / swapSpeed;
        nextFireTime = Time.time + timeToSwap + (fireRate / 2f); 
    }

    void PerformSwap()
    {
        Vector3 targetPos = new Vector3(transform.position.x, targetSwapY, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, swapSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; 
            baseY = targetSwapY;            
            isSwapping = false;             
        }
    }
}