using UnityEngine;

public class StepParticle : MonoBehaviour
{
    public float lifeTime = 0.4f; // Sống trong 0.4 giây rồi biến mất
    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        timer = lifeTime;
        sr.color = Color.white; // Reset màu
        
        // Random góc xoay và kích thước một chút cho tự nhiên
        transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject); // Xóa hạt khi hết thời gian
            return;
        }

        // Bay nhè nhẹ lên trên
        transform.Translate(Vector2.up * 1.5f * Time.deltaTime, Space.World);

        // Mờ dần
        float alpha = timer / lifeTime;
        sr.color = new Color(1, 1, 1, alpha);
    }
}