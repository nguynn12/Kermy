using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalBossHealth : MonoBehaviour
{
    public float maxHealth = 200f;
    public float currentHealth;
    private Slider healthSlider;

    [Header("Death Effects")]
    public AudioClip deathRoarSound; // Tiếng hét lúc chết
    public float fadeDuration = 3f;  // Thời gian mờ dần (Mặc định 3 giây)
    // Biến khóa để đảm bảo Boss không bị chết 2 lần
    private bool isDead = false;

    [Header("Ending Scene")]
    public string endingSceneName = "ENDING";
    public float endingLoadDelay = 0.5f;

    [Header("Winner Sound")]
    
    public AudioClip victoryMusic; // Ô chứa file nhạc lúc thắng game
    

    void Awake()
    {
        currentHealth = maxHealth;

        // Code tìm thanh máu ẩn (như bạn đã làm thành công ở các bước trước)
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            Transform sliderTransform = mainCanvas.transform.Find("BossHealthBar");
            if (sliderTransform != null)
            {
                healthSlider = sliderTransform.GetComponent<Slider>();
                healthSlider.gameObject.SetActive(true);
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }

    // --- THÊM PHẦN NÀY ĐỂ NHẬN SÁT THƯƠNG ---
    // (Bên trong file FinalBossHealth.cs, dưới hàm Start)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có phải đạn của người chơi bắn trúng không
        if (collision.CompareTag("PlayerBullet"))
        {
            TakeDamage(10f); // Trừ 10 máu Boss
            Destroy(collision.gameObject); // Hủy viên đạn của Ếch
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (healthSlider != null) healthSlider.value = currentHealth; // Tụt thanh máu UI
        if (currentHealth <= 0) Die();
    }

   

    void Die()
    {
        isDead = true; // Bật khóa: Đã chết
        Debug.Log("BOSS CUỐI ĐÃ BỊ HẠ GỤC!");
        
        if (healthSlider != null) healthSlider.gameObject.SetActive(false); // Giấu thanh máu đi
        GameObject bgmObject = GameObject.Find("BGM_Manager");
        if (bgmObject != null)
        {
            AudioSource bgmSource = bgmObject.GetComponent<AudioSource>();
            if (bgmSource != null)
            {
                // Tắt ngay lập tức nhạc đánh Boss căng thẳng
                bgmSource.Stop(); 
                
                // Nếu có nhạc chiến thắng thì tráo đĩa và bật lên
                if (victoryMusic != null)
                {
                    bgmSource.clip = victoryMusic; 
                    bgmSource.loop = false; // Nhạc thắng thường chỉ kêu 1 lần rồi thôi
                    bgmSource.Play();       
                }
            }
        }

        // 1. RÚT PHÍCH CẮM NÃO VÀ ÉP DỪNG MỌI CHIÊU THỨC ĐANG NIỆM
        FinalBossAI bossAI = GetComponent<FinalBossAI>();
        if (bossAI != null) 
        {
            bossAI.enabled = false;         // Ngừng hàm Update (Tắt di chuyển, tắt đạn thường)
            bossAI.StopAllCoroutines();     // MỚI THÊM: Ngắt ngay lập tức chiêu cuối đang gồng ngầm!
            
            // MỚI THÊM: Tắt luôn tia ngắm laze (nếu Boss chết đúng lúc đang ngắm)
            if (bossAI.warningLaser != null) bossAI.warningLaser.enabled = false; 
        }

        // 2. TẮT VA CHẠM: Đạn bay xuyên qua
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 3. GẦM THÉT
        if (deathRoarSound != null)
        {
            AudioSource.PlayClipAtPoint(deathRoarSound, Camera.main.transform.position, 1f);
        }

        // 4. CHẠY HIỆU ỨNG MỜ DẦN RỒI XÓA SỔ
        StartCoroutine(FadeOutAndDieRoutine());
        
    }
    IEnumerator FadeOutAndDieRoutine()
    {
        // Lấy bộ vẽ hình ảnh của Boss
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        
        if (sr != null)
        {
            Color originalColor = sr.color;
            float timer = 0f;

            // Giảm Alpha từ 1 về 0 từ từ
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null; 
            }
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // 5. XÓA SỔ BOSS KHỎI GAME (Đã mờ tịt mới xóa)
        if (endingLoadDelay > 0f)
        {
            yield return new WaitForSeconds(endingLoadDelay);
        }

        LoadEndingScene();
    }

    private void LoadEndingScene()
    {
        string sceneToLoad = ResolveSceneNameInBuild(endingSceneName);
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"Ending scene '{endingSceneName}' is not in Build Profiles.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private static string ResolveSceneNameInBuild(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return null;
        }

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (string.Equals(buildSceneName, sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return buildSceneName;
            }
        }

        return null;
    }
}
