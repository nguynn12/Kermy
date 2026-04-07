using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Gắn Action Bản Đồ Phím Vào Đây")]
    public InputActionReference moveAction;
    public InputActionReference interactAction;

    [Header("Input Values")]
    public Vector2 moveInput { get; private set; }
    public bool isActionPressed { get; private set; }

    private void OnEnable()
    {
        // Bật lắng nghe phím khi nhân vật xuất hiện
        if (moveAction != null) moveAction.action.Enable();
        if (interactAction != null) interactAction.action.Enable();
    }

    private void OnDisable()
    {
        // Tắt lắng nghe khi nhân vật bị ẩn/xóa để tránh lỗi bộ nhớ
        if (moveAction != null) moveAction.action.Disable();
        if (interactAction != null) interactAction.action.Disable();
    }

    private void Update()
    {
        if (moveAction != null)
        {
            // Đọc giá trị trục X (trái/phải) và Y (nhảy)
            moveInput = moveAction.action.ReadValue<Vector2>();
        }

        if (interactAction != null)
        {
            // Đọc xem phím tương tác có đang được bấm không
            isActionPressed = interactAction.action.IsPressed();
            
            // Log ra console đúng 1 lần khi vừa bấm xuống
            if (interactAction.action.WasPressedThisFrame())
            {
                Debug.Log(gameObject.name + " thuc hien tuong tac!");
            }
        }
    }
}