using UnityEngine;

public class PlayerRiderStick : MonoBehaviour
{
    [SerializeField] private float minUpNormal = 0.85f; // Tăng lên 0.85 để chỉ dính khi thực sự ở đỉnh đầu (Tránh dính tay)

    [Header("Stick")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float unstickGraceSeconds = 0.1f;
    [SerializeField] private float supportProbeHeight = 0.08f;

    // THÊM: Lấy Rigidbody của người bên dưới để truyền vận tốc
    public Rigidbody2D ParentRigidbody { get; private set; } 

    private Transform _currentParent;
    private Collider2D _col;
    private float _unstickTimer;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnDisable()
    {
        // Đã xóa SetParent(null) để khắc phục triệt để lỗi đỏ Console
        _currentParent = null;
        ParentRigidbody = null;
        _unstickTimer = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_currentParent != null) return;
        TryStick(collision);
    }

    private void FixedUpdate()
    {
        if (_currentParent == null) return;

        if (IsStillSupportedByCurrentParent())
        {
            _unstickTimer = 0f;
            return;
        }

        _unstickTimer += Time.fixedDeltaTime;
        if (_unstickTimer >= unstickGraceSeconds)
        {
            _currentParent = null;
            ParentRigidbody = null;
            _unstickTimer = 0f;
        }
    }

    private void TryStick(Collision2D collision)
    {
        PlayerController otherPlayer = collision.collider.GetComponent<PlayerController>();
        if (otherPlayer == null || !otherPlayer.gameObject.activeInHierarchy) return;

        if (!IsStandingOnTop(collision)) return;

        _currentParent = otherPlayer.transform;
        ParentRigidbody = otherPlayer.GetComponent<Rigidbody2D>(); // Lấy vận tốc người dưới
        _unstickTimer = 0f;
    }

    private bool IsStillSupportedByCurrentParent()
    {
        if (_currentParent == null) return false;
        if (_col == null) return transform.position.y >= _currentParent.position.y;

        Bounds b = _col.bounds;
        // Thu nhỏ chiều rộng hộp quét (0.5f) để nếu trượt ra mép hông là rớt ngay, không bị dính lơ lửng
        Vector2 probeCenter = new Vector2(b.center.x, b.min.y - (supportProbeHeight * 0.5f));
        Vector2 probeSize = new Vector2(b.size.x * 0.5f, supportProbeHeight);

        Collider2D hit = Physics2D.OverlapBox(probeCenter, probeSize, 0f, playerLayer);
        return hit != null && hit.transform == _currentParent;
    }

    private bool IsStandingOnTop(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 n = collision.GetContact(i).normal;
            if (n.y >= minUpNormal) return true;
        }
        return false;
    }
}