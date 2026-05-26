using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    [Header("Cài đặt phím")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public float climbSpeed = 5f;

    [Header("Thoát thang bằng nhảy (Tuỳ chọn)")]
    public KeyCode jumpKey = KeyCode.Space;
    public bool canExitByJump = true;

    private bool _canClimb;    // Đang đứng trong vùng thang
    private bool _isClimbing;  // Đang trong trạng thái leo
    private Rigidbody2D _rb;
    private float _gravityStore;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gravityStore = _rb.gravityScale; // Lưu lại trọng lực gốc
    }

    public void SetCanClimb(bool can)
    {
        _canClimb = can;

        // Khi rời khỏi thang, ép buộc thoát leo và phục hồi trọng lực
        if (!_canClimb)
        {
            StopClimbing();
        }
    }

    private void Update()
    {
        // 1. Bắt đầu leo: Nếu ở trong thang và nhấn phím di chuyển
        if (_canClimb && (Input.GetKey(upKey) || Input.GetKey(downKey)))
        {
            _isClimbing = true;
            _rb.gravityScale = 0f; // Tắt trọng lực khi đang leo
        }

        // 2. Đang trong trạng thái leo
        if (_isClimbing)
        {
            float inputY = 0f;
            if (Input.GetKey(upKey)) inputY = 1f;
            else if (Input.GetKey(downKey)) inputY = -1f;

            // Di chuyển lên/xuống
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, inputY * climbSpeed);

            // Thoát leo bằng phím nhảy (nếu bật)
            if (canExitByJump && Input.GetKeyDown(jumpKey))
            {
                StopClimbing();
            }
        }
        else
        {
            // Đảm bảo trọng lực luôn được trả về nếu không leo
            if (_rb.gravityScale != _gravityStore)
                _rb.gravityScale = _gravityStore;
        }
    }

    // Hàm dùng chung để reset trạng thái
    private void StopClimbing()
    {
        _isClimbing = false;
        _rb.gravityScale = _gravityStore; // Phục hồi trọng lực
        // Có thể thêm lực đẩy nhẹ ra khỏi thang nếu cần
    }
}