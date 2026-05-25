using UnityEngine;

public class BossLaserDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Tia laze mang Tag "BossLazer" nên chiếc khiên xanh sẽ hoàn toàn phớt lờ nó
        
        // 2. Nếu đập trúng người chơi (Ếch)
        if (collision.CompareTag("Player"))
        {
            Level05_PlayerHealth frogHealth = collision.GetComponent<Level05_PlayerHealth>();
            if (frogHealth != null)
            {
                // Gọi hàm trừ máu của người chơi
                frogHealth.TakeDamage();
                Debug.Log("Ếch đã bị dính tia Laze hủy diệt!");
            }
        }
    }
}