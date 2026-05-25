using UnityEngine;

public class FinalBossAI : MonoBehaviour
{
    [Header("Phase Control")]
    public int currentPhase = 1;
    private Animator anim;

    [Header("Fire Points (Vị trí nòng súng)")]
    public Transform leftArmFirePoint;
    public Transform rightArmFirePoint;
    public Transform centerFirePoint; // Dành cho miệng/lõi

    [Header("Weapons (Vũ khí)")]
    public GameObject purpleBulletPrefab; // Vat_the_ban_Final_boss
    public GameObject energyBallPrefab;   // Final_Boss_EnergyBall
    
    [Header("Attack Speeds")]
    public float phase1FireRate = 2f;
    public float phase2FireRate = 1.5f; // Bắn nhanh hơn khi tức giận
    private float nextFireTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        nextFireTime = Time.time + phase1FireRate;
    }

    void Update()
    {
        if (currentPhase == 1)
        {
            Phase1Behavior();
        }
        else if (currentPhase == 2)
        {
            Phase2Behavior();
        }
        // Phase 3 (Ultimate) bỏ trống chờ bạn ra lệnh
    }

    // --- PHASE 1 (HP 200 -> 150): Bắn đạn tím 2 tay ---
    void Phase1Behavior()
    {
        if (Time.time >= nextFireTime)
        {
            if (anim != null) anim.SetTrigger("attack");

            // Bắn đạn tím từ cả 2 tay
            if (purpleBulletPrefab != null)
            {
                if (leftArmFirePoint != null) Instantiate(purpleBulletPrefab, leftArmFirePoint.position, Quaternion.identity);
                if (rightArmFirePoint != null) Instantiate(purpleBulletPrefab, rightArmFirePoint.position, Quaternion.identity);
            }
            nextFireTime = Time.time + phase1FireRate;
        }
    }

    // --- PHASE 2 (HP 150 -> 100): 2 đạn tím + 3 cầu năng lượng ---
    void Phase2Behavior()
    {
        if (Time.time >= nextFireTime)
        {
            if (anim != null) anim.SetTrigger("attack");

            // 1. Vẫn bắn đạn tím từ 2 tay
            if (purpleBulletPrefab != null)
            {
                if (leftArmFirePoint != null) Instantiate(purpleBulletPrefab, leftArmFirePoint.position, Quaternion.identity);
                if (rightArmFirePoint != null) Instantiate(purpleBulletPrefab, rightArmFirePoint.position, Quaternion.identity);
            }

            // 2. Bắn 3 quả cầu năng lượng tỏa ra từ giữa (Center)
            if (energyBallPrefab != null && centerFirePoint != null)
            {
                // Bắn thẳng
                Instantiate(energyBallPrefab, centerFirePoint.position, Quaternion.identity);
                
                // Bắn xéo lên (xoay 15 độ)
                Instantiate(energyBallPrefab, centerFirePoint.position, Quaternion.Euler(0, 0, 15f));
                
                // Bắn xéo xuống (xoay -15 độ)
                Instantiate(energyBallPrefab, centerFirePoint.position, Quaternion.Euler(0, 0, -15f));
            }
            nextFireTime = Time.time + phase2FireRate;
        }
    }

    public void ChangePhase(int newPhase)
    {
        // Tránh gọi lại Phase đang chạy
        if (currentPhase == newPhase) return; 

        currentPhase = newPhase;
        if (anim != null) anim.SetInteger("form", currentPhase);
        Debug.Log("BOSS CHUYỂN SANG PHASE: " + currentPhase);
    }
}