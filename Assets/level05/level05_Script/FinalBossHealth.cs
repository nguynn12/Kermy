using UnityEngine;
using UnityEngine.UI;

public class FinalBossHealth : MonoBehaviour
{
    public float maxHealth = 200f;
    public float currentHealth;
    private Slider healthSlider;

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
        currentHealth -= damage;
        if (healthSlider != null) healthSlider.value = currentHealth; // Tụt thanh máu UI
        if (currentHealth <= 0) Die();
    }

   

    void Die()
    {
        Debug.Log("BOSS CUỐI ĐÃ BỊ HẠ GỤC!");
        if (healthSlider != null) healthSlider.gameObject.SetActive(false); // Giấu thanh máu đi
        Destroy(gameObject); // Xóa Boss (Sau này sẽ thêm hoạt ảnh nổ tung hoành tráng)
    }
}