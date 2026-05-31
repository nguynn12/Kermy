using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Tốc độ di chuyển")]
    public float speed = 4f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCombatHealth playerHealth = collision.GetComponent<PlayerCombatHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            Destroy(gameObject);
        }
    }
}