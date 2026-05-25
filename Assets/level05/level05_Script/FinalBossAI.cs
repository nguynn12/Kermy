using UnityEngine;
using System.Collections; 

public class FinalBossAI : MonoBehaviour
{
    [Header("Di Chuyển")]
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
        startY = targetPosition.y;
        
        if (warningLaser != null) warningLaser.enabled = false;
    }

    void Update()
    {
        if (!hasReachedPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                hasReachedPosition = true;
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

            // --- 2. GIAI ĐOẠN KHAI HỎA (Bắn Laze thật - 4 giây) ---
            if (laserPrefab != null && centerFirePoint != null)
            {
                activeLaser = Instantiate(laserPrefab, centerFirePoint.position, Quaternion.identity);
                activeLaser.transform.SetParent(centerFirePoint); 

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
        }
    }
}