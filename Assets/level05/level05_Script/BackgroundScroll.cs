using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Tốc độ cuộn")]
    public float scrollSpeed = 2f;

    private float length;
    private Vector3 startPosition;

    void Start()
    {
        // Lưu lại vị trí ban đầu của ảnh
        startPosition = transform.position;
        
        // Tự động đo chiều dài của bức ảnh nền
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Di chuyển ảnh sang trái
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Nếu ảnh đã trôi qua trái một đoạn bằng đúng chiều dài của nó
        if (startPosition.x - transform.position.x >= length)
        {
            // Dịch chuyển nó quay trở lại vị trí nối đuôi
            transform.position = startPosition;
        }
    }
}