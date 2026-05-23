using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Cài đặt Đạn")]
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Trúng quái hoặc chướng ngại vật
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject); 
            Destroy(gameObject);           
        }
        
        // 2. Trúng đồng đội (Friendly Fire)
        else if (collision.CompareTag("Player"))
        {
            Level05_PlayerHealth playerHealth = collision.GetComponent<Level05_PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); 
            }

            Destroy(gameObject); 
        }
    }
}