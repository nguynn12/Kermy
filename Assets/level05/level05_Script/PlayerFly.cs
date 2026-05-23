using UnityEngine;

public class PlayerFly : MonoBehaviour
{
    [Header("Cài đặt tốc độ bay")]
    public float flySpeed = 5f;

    [Header("Cài đặt phím điều khiển")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("Giới hạn màn hình (Để nhân vật không bay ra ngoài)")]
    public float minX = -8.5f;
    public float maxX = 8.5f;
    public float minY = -4.5f;
    public float maxY = 4.5f;

    void Update()
    {
        // Tạo biến lưu trữ hướng bay
        Vector3 movement = Vector3.zero;

        // Kiểm tra người chơi bấm phím nào
        if (Input.GetKey(upKey))
        {
            movement.y += 1f;
        }
        if (Input.GetKey(downKey))
        {
            movement.y -= 1f;
        }
        if (Input.GetKey(leftKey))
        {
            movement.x -= 1f;
        }
        if (Input.GetKey(rightKey))
        {
            movement.x += 1f;
        }

        // Chuẩn hóa vector để bay chéo không bị nhanh hơn bay thẳng
        movement = movement.normalized;

        // Di chuyển nhân vật
        transform.position += movement * flySpeed * Time.deltaTime;

        // Khóa vị trí nhân vật không cho bay ra khỏi khung hình camera
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}