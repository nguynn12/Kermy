using UnityEngine;

public class FinalBossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 200f;
    public float currentHealth;

    private FinalBossAI bossAI;

    void Start()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<FinalBossAI>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("FINAL BOSS HP: " + currentHealth);

        // Kiểm tra mốc đổi Phase
        if (currentHealth <= 150f && currentHealth > 100f)
        {
            bossAI.ChangePhase(2); // Phase kết hợp cầu năng lượng
        }
        else if (currentHealth <= 100f && currentHealth > 0)
        {
            bossAI.ChangePhase(3); // Phase Ultimate (Sẽ làm sau)
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("FINAL BOSS DEFEATED!");
        Destroy(gameObject);
        // Sau này có thể thêm hiệu ứng nổ hoặc chuyển sang màn hình Win Game
    }
}