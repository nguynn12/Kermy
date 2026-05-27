using UnityEngine;
using System.Collections; 

public class FinalBossAI : MonoBehaviour
{
    [Header("Âm Thanh Tấn Công")]
    public AudioClip purpleMissileSound; // Ô chứa file tiếng phóng tên lửa
    // (Đảm bảo bạn đã có biến audioSource và đã nạp nó trong hàm Start() nhé)
    public AudioClip energyBallSound;
    public AudioClip laserChargeSound;   // Tiếng gồng tia laze (MỚI)
    public AudioClip laserFireSound;     // Tiếng nổ/phóng tia laze (MỚI)
    private AudioSource audioSource;

    [Header("Di Chuyển")]
    public float smoothTime = 0.5f; // Thời gian trượt (càng lớn trượt càng chậm và mượt)
    private Vector3 velocity = Vector3.zero; // Biến ẩn để Unity tự tính toán gia tốc
    public float moveSpeed = 3f;
    public Vector3 targetPosition = new Vector3(8f, 0f, 0f); 
    public float hoverSpeed = 2f; 
    public float hoverRange = 3f; 
    private float startY; 
    private bool hasReachedPosition = false;

    [Header("Phase Control")]
    public int currentPhase = 1;

    [Header("Fire Points")]
    public Transform leftArmFirePoint;
    public Transform rightArmFirePoint;
    public Transform centerFirePoint; 

    [Header("Weapons")]
    public GameObject purpleBulletPrefab;
    public GameObject energyBallPrefab;
    public GameObject laserPrefab; 
    
    [Header("Warning Laser (Tia ngắm)")]
    public LineRenderer warningLaser; 

    [Header("Attack Speeds")]
    public float phase1FireRate = 2f;
    public float phase2FireRate = 1f;

    private float nextFireTime;
    private Animator anim;
    private FinalBossHealth bossHealth;
    private bool isLaserRoutineStarted = false; 
    private GameObject activeLaser; 
    
    // Biến đánh dấu: True khi đang gồng/bắn Laze, False khi đang nghỉ
    private bool isCastingLaser = false; 

    void Start()
    {
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<FinalBossHealth>();


        // KẾT NỐI LOA: Tìm cái AudioSource bạn đã gắn trên người con Boss
        audioSource = GetComponent<AudioSource>();

        startY = targetPosition.y;
        
        if (warningLaser != null) warningLaser.enabled = false;
    }

    void Update()
    {
        if (!hasReachedPosition)
        {
            // Nâng cấp: Dùng SmoothDamp để Boss lướt về vị trí siêu mượt và giảm tốc khi gần tới
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Kiểm tra xem đã tới nơi chưa (Do SmoothDamp trượt rất êm nên khoảng cách check để hơi lớn xíu)
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition; // Chốt cứng tọa độ cho chuẩn xác
                hasReachedPosition = true;
                
                // Bật lại khung va chạm, chính thức nhận sát thương
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = true; 

                if (anim != null) anim.SetTrigger("startCombat");
                nextFireTime = Time.time + 2.5f;
            }
        }
        else 
        {
            // CHỈ lượn lên xuống nếu KHÔNG trong trạng thái bắn Laze
            if (!isCastingLaser)
            {
                float newY = startY + Mathf.Sin(Time.time * hoverSpeed) * hoverRange;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            if (bossHealth != null && bossHealth.currentHealth <= 150 && bossHealth.currentHealth > 100 && currentPhase == 1)
            {
                currentPhase = 2;
            }

            if (bossHealth != null && bossHealth.currentHealth <= 100 && currentPhase != 3)
            {
                currentPhase = 3;
                if (anim != null) anim.SetTrigger("Phase3"); 
            }

            if (Time.time >= nextFireTime)
            {
                if (currentPhase == 1)
                {
                    ShootPurpleBullets();
                    nextFireTime = Time.time + phase1FireRate;
                }
                else if (currentPhase == 2)
                {
                    ShootEnergyBalls(); 
                    nextFireTime = Time.time + phase2FireRate;
                }
                else if (currentPhase == 3)
                {
                    if (!isLaserRoutineStarted)
                    {
                        StartCoroutine(LaserAttackRoutine());
                    }
                }
            }
        }
    }

    IEnumerator LaserAttackRoutine()
    {
        isLaserRoutineStarted = true;

        // Chờ Boss diễn xong hoạt ảnh há miệng biến hình ban đầu
        yield return new WaitForSeconds(1.5f);

        while (currentPhase == 3)
        {
            // KHÓA VỊ TRÍ: Ép Boss đứng yên ngay khi bắt đầu chu kỳ chiêu cuối
            isCastingLaser = true; 

            // --- 1. GIAI ĐOẠN CẢNH BÁO (Ngắm 1.5 giây) ---
            if (warningLaser != null) warningLaser.enabled = true;
            // PHÁT TIẾNG GỒNG LAZE (Hú còi, sạc năng lượng...)
            if (laserChargeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(laserChargeSound, 1f);
            }

            float chargeTime = 1.5f;
            float timer = 0f;

            while (timer < chargeTime)
            {
                if (warningLaser != null && centerFirePoint != null)
                {
                    warningLaser.SetPosition(0, centerFirePoint.position);
                    warningLaser.SetPosition(1, centerFirePoint.position + Vector3.left * 40f); 
                }
                timer += Time.deltaTime;
                yield return null; 
            }

            if (warningLaser != null) warningLaser.enabled = false;

            
         // --- 2. GIAI ĐOẠN KHAI HỎA (BẮN LAZE THẬT) ---
            if (laserPrefab != null && centerFirePoint != null)
            {
                // CHỈNH SỬA TẠI ĐÂY: Tắt sạch tiếng gồng cũ đang phát dở trước khi nổ tiếng laze mới
                if (audioSource != null)
                {
                    audioSource.Stop(); 
                }

                activeLaser = Instantiate(laserPrefab, centerFirePoint.position, Quaternion.identity);
                activeLaser.transform.SetParent(centerFirePoint); 
                // 4 DÒNG CODE PHÁT TIẾNG SẠC LAZE LÀ Ở ĐÂY:
            if (laserChargeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(laserChargeSound, 1f);
            }

                // Phát tiếng nổ tia laze dứt khoát
                if (laserFireSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(laserFireSound, 1f); 
                }

                yield return new WaitForSeconds(4f);

                Animator laserAnim = activeLaser.GetComponent<Animator>();
                if (laserAnim != null) laserAnim.SetTrigger("StopLaser");

                yield return new WaitForSeconds(0.6f);
                Destroy(activeLaser);
            }

            // MỞ KHÓA VỊ TRÍ: Boss bắn xong rồi, cho phép lượn lên xuống trở lại trong lúc hồi chiêu
            isCastingLaser = false; 

            // --- 3. GIAI ĐOẠN NGHỈ NGƠI (Hồi chiêu) ---
            yield return new WaitForSeconds(2.5f); 
        }
    }

    void ShootPurpleBullets()
    {
        if (purpleBulletPrefab != null)
        {
            if (leftArmFirePoint != null) Instantiate(purpleBulletPrefab, leftArmFirePoint.position, Quaternion.identity);
            if (rightArmFirePoint != null) Instantiate(purpleBulletPrefab, rightArmFirePoint.position, Quaternion.identity);
        }
        // KÍCH HOẠT ÂM THANH PHÓNG TÊN LỬA TÍM TẠI ĐÂY
        if (purpleMissileSound != null && audioSource != null)
        {
            // Phát âm thanh với mức âm lượng 80% (0.8f) để không quá chói tai
            audioSource.PlayOneShot(purpleMissileSound, 0.8f); 
        }
    }

    void ShootEnergyBalls()
    {
        ShootPurpleBullets();
        if (energyBallPrefab != null && centerFirePoint != null)
        {
            float[] angles = { 15f, 0f, -15f };
            for (int i = 0; i < angles.Length; i++)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angles[i]);
                Instantiate(energyBallPrefab, centerFirePoint.position, rotation);
            }
              // KÍCH HOẠT ÂM THANH QUẢ CẦU NĂNG LƯỢNG (MỚI THÊM)
            if (energyBallSound != null && audioSource != null)
            {
                // Phát tiếng quả cầu năng lượng, có thể tăng volume lên 1f nếu muốn uy lực hơn
                audioSource.PlayOneShot(energyBallSound, 0.9f); 
            }
        }
      
    }
}