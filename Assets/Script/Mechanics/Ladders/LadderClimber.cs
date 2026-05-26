using UnityEngine;

// Gắn vào Ignus VÀ Aqua
public class LadderClimber : MonoBehaviour
{
    private bool _onLadder = false;
    private float _climbSpeed = 5f;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!_onLadder) return;

        // Đọc input theo Player — Player1: W/S, Player2: ↑/↓
        float vertical = 0f;

        // Thử đọc từ PlayerInputHandler nếu có
        var input = GetComponent<PlayerInputHandler>();
        if (input != null)
        {
            vertical = input.MoveInput.y;
            // MoveInput.y = -1 (xuống), 0 (đứng), 1 (lên)
        }

        if (Mathf.Abs(vertical) > 0.1f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, vertical * _climbSpeed);
            AudioManager.Instance?.PlayLadder();
        }
        else
        {
            // Không bấm → đứng yên trên thang (không rơi)
            _rb.linearVelocity = new Vector2(
                _rb.linearVelocity.x,
                0f
            );
        }
    }

    public void EnterLadder(float speed)
    {
        _onLadder = true;
        _climbSpeed = speed;
        _rb.gravityScale = 0f;
        // Tắt gravity → không rơi khi trên thang
        _rb.linearVelocity = Vector2.zero;
    }

    public void ExitLadder()
    {
        _onLadder = false;
        _rb.gravityScale = 1f;
        // Bật lại gravity bình thường
    }
}