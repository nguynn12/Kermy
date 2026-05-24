using UnityEngine;

// Gắn vào Ignus VÀ Aqua
// Script này xử lý việc leo thang phía nhân vật
public class LadderClimber : MonoBehaviour
{
    // ── Biến nội bộ ───────────────────────────────────────────
    private bool _onLadder = false;
    // true = đang đứng trong vùng thang

    private float _climbSpeed = 4f;
    // Tốc độ leo, nhận từ Ladder script

    private Rigidbody2D _rb;
    private PlayerInputHandler _input;
    // PlayerInputHandler = script đọc input có sẵn trong project

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!_onLadder) return;
        // Không trong thang → không làm gì

        // Đọc input nhảy (Jump button)
        // Kiểm tra tên action trong PlayerInputHandler của nhóm bạn
        bool jumpPressed = _input != null && IsJumpPressed();

        if (jumpPressed)
        {
            // Nhấn nhảy trong thang → bay lên
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _climbSpeed * 2f);
            // Nhân 2 để cảm giác "đẩy" lên mạnh hơn leo từ từ
        }
    }

    private bool IsJumpPressed()
    {
        // Thử đọc input từ PlayerInputHandler
        // Nếu không dùng được thì dùng Input.GetKeyDown thay thế
        return Input.GetKeyDown(KeyCode.Space)   // Player 1
            || Input.GetKeyDown(KeyCode.UpArrow); // Player 2
        // Bạn chỉnh lại phím cho đúng với game của nhóm
    }

    // ── Gọi từ Ladder script ──────────────────────────────────
    public void EnterLadder(float speed)
    {
        _onLadder = true;
        _climbSpeed = speed;

        // Tắt gravity khi trên thang
        // Nếu không tắt, nhân vật sẽ rơi xuống ngay
        _rb.gravityScale = 0f;

        // Dừng velocity hiện tại
        _rb.linearVelocity = Vector2.zero;
    }

    public void ExitLadder()
    {
        _onLadder = false;

        // Bật lại gravity bình thường
        _rb.gravityScale = 1f;
    }
}