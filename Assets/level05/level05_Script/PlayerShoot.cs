using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Âm Thanh")]
    public AudioClip shootSound; // Chứa file tiếng súng
    private AudioSource audioSource; // Chứa cái loa mình vừa gắn
    
    [Header("Vũ khí")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    [Header("Cài đặt Bắn")]
    public KeyCode shootKey;        
    public float fireRate = 0.3f;   
    
    private float nextFireTime = 0f;

    void Start()
    {
        // Hệ thống tự động tìm và kết nối với cái loa (Audio Source) đã gắn trên con ếch
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(shootKey) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        // 1. Lệnh đẻ ra viên đạn
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2. Lệnh phát ra tiếng súng ngay lúc đẻ đạn
        if (shootSound != null && audioSource != null)
        {
            // PlayOneShot giúp tiếng súng kêu lên mà không làm tắt tiếng bay lơ lửng
            audioSource.PlayOneShot(shootSound); 
        }
    }
}