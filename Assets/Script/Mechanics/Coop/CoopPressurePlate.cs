using UnityEngine;

public class CoopPressurePlate : MonoBehaviour
{
    [Header("Cấu hình Nút bấm")]
    public Sprite unpressedButton;
    public Sprite pressedButton;

    [Header("Cấu hình Cửa")]
    public GameObject targetDoor;
    public Sprite doorClosedSprite; // Kéo hình cửa đóng vào đây
    public Sprite doorOpenSprite;   // Kéo hình cửa mở vào đây

    private SpriteRenderer buttonRenderer;
    private SpriteRenderer doorRenderer;
    private Collider2D doorCollider;
    private int playersOnButton = 0;

    void Start()
    {
        buttonRenderer = GetComponent<SpriteRenderer>();

        if (targetDoor != null)
        {
            // Lấy các thành phần của cánh cửa để điều khiển
            doorRenderer = targetDoor.GetComponent<SpriteRenderer>();
            doorCollider = targetDoor.GetComponent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CoopPlayerMovement>() != null)
        {
            playersOnButton++;
            if (playersOnButton == 1)
            {
                // 1. Đổi hình nút thành "Đã nhấn"
                if (pressedButton != null) buttonRenderer.sprite = pressedButton;

                // 2. Mở cửa: Đổi hình và tắt vật cản
                if (doorRenderer != null && doorOpenSprite != null)
                    doorRenderer.sprite = doorOpenSprite;

                if (doorCollider != null) doorCollider.enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CoopPlayerMovement>() != null)
        {
            playersOnButton--;
            if (playersOnButton <= 0)
            {
                playersOnButton = 0;
                // 1. Đổi hình nút thành "Chưa nhấn"
                if (unpressedButton != null) buttonRenderer.sprite = unpressedButton;

                // 2. Đóng cửa: Trả lại hình đóng và bật vật cản
                if (doorRenderer != null && doorClosedSprite != null)
                    doorRenderer.sprite = doorClosedSprite;

                if (doorCollider != null) doorCollider.enabled = true;
            }
        }
    }
}