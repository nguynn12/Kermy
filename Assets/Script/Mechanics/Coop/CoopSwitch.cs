using UnityEngine;

public class CoopSwitch : MonoBehaviour
{
    [Header("Cấu hình Sprites")]
    public Sprite switchOffSprite; // Cần gạt vị trí 1
    public Sprite switchOnSprite;  // Cần gạt vị trí 2

    [Header("Liên kết Cửa")]
    public GameObject targetDoor;
    public Sprite doorClosedSprite;
    public Sprite doorOpenSprite;

    private SpriteRenderer switchRenderer;
    private SpriteRenderer doorRenderer;
    private Collider2D doorCollider;
    private bool isOn = false; // Trạng thái hiện tại của cần gạt

    void Start()
    {
        switchRenderer = GetComponent<SpriteRenderer>();
        if (targetDoor != null)
        {
            doorRenderer = targetDoor.GetComponent<SpriteRenderer>();
            doorCollider = targetDoor.GetComponent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu là Player chạm vào
        if (collision.GetComponent<CoopPlayerMovement>() != null)
        {
            ToggleSwitch();
        }
    }

    void ToggleSwitch()
    {
        isOn = !isOn; // Đảo trạng thái (đang tắt thành bật và ngược lại)

        if (isOn)
        {
            if (switchOnSprite != null) switchRenderer.sprite = switchOnSprite;
            if (doorRenderer != null && doorOpenSprite != null) doorRenderer.sprite = doorOpenSprite;
            if (doorCollider != null) doorCollider.enabled = false; // Mở cửa
        }
        else
        {
            if (switchOffSprite != null) switchRenderer.sprite = switchOffSprite;
            if (doorRenderer != null && doorClosedSprite != null) doorRenderer.sprite = doorClosedSprite;
            if (doorCollider != null) doorCollider.enabled = true; // Đóng cửa
        }
    }
}