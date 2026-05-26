using UnityEngine;

public class BossLaserDamage : MonoBehaviour
{
    // Dùng OnTriggerEnter2D để bắt va chạm NGAY TÍCH TẮC lúc Ếch vừa đụng vào Laze
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Level05_PlayerHealth frogHealth = collision.GetComponent<Level05_PlayerHealth>();
            if (frogHealth != null)
            {
                // Gọi TakeDamage 2 lần liên tiếp để rút cạn 2 máu của Ếch ngay trong 1 khung hình!
                frogHealth.TakeDamage();
                frogHealth.TakeDamage(); 
                
                Debug.Log("Ếch đã bị Laze hóa vàng ngay lập tức!");
            }
        }
    }

    // Dùng thêm OnTriggerStay2D đề phòng Ếch bay vào lúc Laze đang bắn dở
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Level05_PlayerHealth frogHealth = collision.GetComponent<Level05_PlayerHealth>();
            if (frogHealth != null)
            {
                frogHealth.TakeDamage();
                frogHealth.TakeDamage(); 
            }
        }
    }
}