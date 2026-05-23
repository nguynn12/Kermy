using UnityEngine;
using UnityEngine.SceneManagement;

public class Level05_PlayerHealth : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    private int hitCount = 0;          
    private float recoveryTimer = 0f;  
    private float recoveryTime = 5f;   

    // TẠO CHỐT CHẶN: Đánh dấu xem game đã bắt đầu quá trình reset chưa
    private bool isResetting = false; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            
            // SỬ DỤNG INVOKE ĐỂ TRÌ HOÃN LỆNH RESET 0.1 GIÂY
            // Giúp Unity có đủ thời gian thoát khỏi luồng tính toán va chạm vật lý
            Invoke("RestartLevel", 0.1f);
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