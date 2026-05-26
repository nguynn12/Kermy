using UnityEngine;
using UnityEngine.SceneManagement;

public class Level05_PlayerHealth : MonoBehaviour 
{
    [Header("Death Sound")]
    public AudioClip deathSound; 
    private AudioSource audioSource; 

    private SpriteRenderer spriteRenderer;
    private int hitCount = 0;          
    private float recoveryTimer = 0f;  
    private float recoveryTime = 5f;   

    // TẠO CHỐT CHẶN: Đánh dấu xem game đã bắt đầu quá trình reset chưa
    private bool isResetting = false; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); // Tìm loa đã gắn trên nhân vật
    }

    void Update()
    {
        if (hitCount == 1 && !isResetting)
        {
            recoveryTimer += Time.deltaTime;
            
            if (recoveryTimer >= recoveryTime)
            {
                ResetStatus();
            }
        }
    }

    public void TakeDamage()
    {
        if (isResetting) return; 

        hitCount++;

        if (hitCount == 1)
        {
            Color c = spriteRenderer.color;
            c.a = 0.5f; 
            spriteRenderer.color = c;

            recoveryTimer = 0f; 
            Debug.Log(gameObject.name + " đã dính đạn lần 1! Mờ đi 50%.");
        }
        else if (hitCount >= 2)
        {
            isResetting = true; 
            Debug.Log(gameObject.name + " dính đạn lần 2! Chuẩn bị reset màn chơi...");
            
            // 1. Phát tiếng âm thanh khi chết
            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            // 2. Ẩn hình ảnh con ếch đi tạo cảm giác "bốc hơi"
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // 3. Tắt Collider để không bị vướng vào các đạn khác đang bay tới
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 4. Kéo dài thời gian trì hoãn lên 1.5 giây để tiếng chết kịp phát hết trước khi tải lại Scene
            Invoke("RestartLevel", 0.4f);
        }
    }

    void ResetStatus()
    {
        hitCount = 0;
        recoveryTimer = 0f;

        Color c = spriteRenderer.color;
        c.a = 1f; 
        spriteRenderer.color = c;
        Debug.Log(gameObject.name + " đã hồi phục hoàn toàn.");
    }

    void RestartLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}