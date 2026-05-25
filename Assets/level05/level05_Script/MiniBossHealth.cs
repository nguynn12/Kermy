using UnityEngine;
using System.Collections; // Cần thiết để dùng IEnumerator (hiệu ứng thời gian)

public class MiniBossHealth : MonoBehaviour
{
    // Khai báo danh sách các hệ nguyên tố
    public enum Element { Fire, Water, None }

    [Header("Cài đặt thuộc tính Boss")]
    public Element bossElement; // Boss này thuộc hệ gì?
    public float maxHealth = 50f; // Máu tối đa
    
    private float currentHealth;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Gán máu đầy khi Boss vừa sinh ra
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Hàm nhận sát thương (sẽ được gọi khi viên đạn chạm vào Boss)
    public void TakeDamage(float damageAmount, Element bulletElement)
    {
        float finalDamage = damageAmount;

        // --- CƠ CHẾ KHẮC HỆ ---
        
        // 1. Boss Nước trúng Đạn Lửa -> x2 Sát thương
        if (bossElement == Element.Water && bulletElement == Element.Fire)
        {
            finalDamage *= 2f;
            Debug.Log("Sát thương x2! Đạn Lửa khắc chế Rồng Nước!");
            StartCoroutine(FlashColor(Color.red)); // Nháy màu đỏ
        }
        // 2. Boss Lửa trúng Đạn Nước -> x2 Sát thương
        else if (bossElement == Element.Fire && bulletElement == Element.Water)
        {
            finalDamage *= 2f;
            Debug.Log("Sát thương x2! Đạn Nước khắc chế Rồng Lửa!");
            StartCoroutine(FlashColor(Color.cyan)); // Nháy màu xanh ngọc
        }
        else
        {
            // 3. Trúng đạn cùng hệ hoặc không khắc chế -> Sát thương bình thường
            StartCoroutine(FlashColor(Color.white));
        }

        // Trừ máu
        currentHealth -= finalDamage;
        Debug.Log(gameObject.name + " còn lại: " + currentHealth + " máu");

        // Kiểm tra xem Boss đã "bay màu" chưa
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hiệu ứng nháy màu báo hiệu bị trúng đạn (kéo dài 0.1 giây)
    IEnumerator FlashColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");
        
        // (Tạm thời cho Boss biến mất. Sau này làm Wave 4 thì bạn sẽ gọi Boss hợp thể ở đoạn này)
        Destroy(gameObject);
    }
}