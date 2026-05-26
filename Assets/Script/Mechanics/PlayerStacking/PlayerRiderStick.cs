using UnityEngine;

public class PlayerRiderStick : MonoBehaviour
{
    [SerializeField] private float minUpNormal = 0.85f;

    [Header("Stick")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float unstickGraceSeconds = 0.1f;
    [SerializeField] private float supportProbeHeight = 0.08f;

    public Rigidbody2D ParentRigidbody { get; private set; }

    private Transform _currentParent;
    private Rigidbody2D _rb;
    private Collider2D _col;
    private float _unstickTimer;
    private Vector2 _lastParentPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void OnDisable()
    {
        ClearParent();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_currentParent != null)
        {
            return;
        }

        TryStick(collision);
    }

    private void FixedUpdate()
    {
        if (_currentParent == null)
        {
            return;
        }

        if (IsStillSupportedByCurrentParent())
        {
            FollowParentMotion();
            _unstickTimer = 0f;
            return;
        }

        _unstickTimer += Time.fixedDeltaTime;
        if (_unstickTimer >= unstickGraceSeconds)
        {
            ClearParent();
        }
    }

    private void TryStick(Collision2D collision)
    {
        PlayerController otherPlayer = collision.collider.GetComponent<PlayerController>();
        if (otherPlayer == null || !otherPlayer.gameObject.activeInHierarchy)
        {
            return;
        }

        if (!IsStandingOnTop(collision))
        {
            return;
        }

        _currentParent = otherPlayer.transform;
        ParentRigidbody = otherPlayer.GetComponent<Rigidbody2D>();
        _lastParentPosition = ParentRigidbody != null ? ParentRigidbody.position : (Vector2)_currentParent.position;
        _unstickTimer = 0f;
    }

    private void FollowParentMotion()
    {
        if (_rb == null || _currentParent == null)
        {
            return;
        }

        Vector2 currentParentPosition = ParentRigidbody != null ? ParentRigidbody.position : (Vector2)_currentParent.position;
        Vector2 delta = currentParentPosition - _lastParentPosition;
        if (delta != Vector2.zero)
        {
            _rb.position += delta;
        }

        _lastParentPosition = currentParentPosition;
    }

    private bool IsStillSupportedByCurrentParent()
    {
        if (_currentParent == null)
        {
            return false;
        }

        if (_col == null)
        {
            return transform.position.y >= _currentParent.position.y;
        }

        Bounds b = _col.bounds;
        Vector2 probeCenter = new Vector2(b.center.x, b.min.y - (supportProbeHeight * 0.5f));
        Vector2 probeSize = new Vector2(b.size.x * 0.72f, supportProbeHeight);

        Collider2D hit = Physics2D.OverlapBox(probeCenter, probeSize, 0f, ResolvePlayerLayer());
        return hit != null && hit.transform == _currentParent;
    }

    private bool IsStandingOnTop(Collision2D collision)
    {
        Collider2D otherCollider = collision.collider;
        if (_col != null && otherCollider != null)
        {
            Bounds self = _col.bounds;
            Bounds other = otherCollider.bounds;
            bool aboveOther = self.min.y >= other.center.y;
            bool closeToTop = Mathf.Abs(self.min.y - other.max.y) <= 0.25f;
            bool horizontallyOverlapping = self.min.x < other.max.x && self.max.x > other.min.x;

            if (aboveOther && closeToTop && horizontallyOverlapping)
            {
                return true;
            }
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y >= minUpNormal)
            {
                return true;
            }
        }

        return false;
    }

    private LayerMask ResolvePlayerLayer()
    {
        if (playerLayer.value != 0)
        {
            return playerLayer;
        }

        int playerLayerIndex = LayerMask.NameToLayer("Player");
        return playerLayerIndex >= 0 ? 1 << playerLayerIndex : Physics2D.AllLayers;
    }

    private void ClearParent()
    {
        _currentParent = null;
        ParentRigidbody = null;
        _unstickTimer = 0f;
        _lastParentPosition = Vector2.zero;
    }
}
