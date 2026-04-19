using UnityEngine;
using System.Collections.Generic; // Đừng quên dòng này để dùng List

[RequireComponent(typeof(Rigidbody2D))]
public class PushableBox : MonoBehaviour
{
    public enum BoxType { Normal, Heavy, Ice, Lava }

    [Header("Type")]
    [SerializeField] private BoxType boxType = BoxType.Normal;

    [Header("Heavy Settings")]
    [SerializeField] private float heavyRequiredDirectionDot = 0.75f;

    [Header("Element Settings")]
    [SerializeField] private ElementalType requiredElementForElementBox = ElementalType.Water;

    private Rigidbody2D _rb;
    private RigidbodyConstraints2D _baseConstraints;

    // SỬA ĐỔI: Dùng List để biết chính xác AI đang chạm trực tiếp vào hộp
    private List<PlayerController> _activePushers = new List<PlayerController>();
    private int _qualifiedElementPushersCount;
    private Vector2 _sumPushDirections;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _baseConstraints = _rb.constraints;
        if (boxType == BoxType.Ice) requiredElementForElementBox = ElementalType.Water;
        else if (boxType == BoxType.Lava) requiredElementForElementBox = ElementalType.Fire;
    }

    private void FixedUpdate()
    {
        ApplyConstraintsForType();
        
        // Reset dữ liệu mỗi khung hình vật lý
        _activePushers.Clear();
        _qualifiedElementPushersCount = 0;
        _sumPushDirections = Vector2.zero;
    }

    private void ApplyConstraintsForType()
    {
        RigidbodyConstraints2D constraints = _baseConstraints;
        switch (boxType)
        {
            case BoxType.Normal:
                break;
            case BoxType.Heavy:
                if (!ShouldAllowHeavyMovement()) constraints |= RigidbodyConstraints2D.FreezePositionX;
                break;
            case BoxType.Ice:
            case BoxType.Lava:
                if (_qualifiedElementPushersCount == 0) constraints |= RigidbodyConstraints2D.FreezePositionX;
                break;
        }
        _rb.constraints = constraints;
    }

    private bool ShouldAllowHeavyMovement()
    {
        // TRƯỜNG HỢP 1: Cả 2 người cùng trực tiếp chạm mặt hộp
        if (_activePushers.Count >= 2)
        {
            Vector2 combined = _sumPushDirections.normalized;
            return Mathf.Abs(combined.x) >= heavyRequiredDirectionDot;
        }

        // TRƯỜNG HỢP 2: ĐẨY NỐI ĐUÔI (1 người chạm hộp, 1 người ủn mông phía sau)
        if (_activePushers.Count == 1)
        {
            PlayerController frontPlayer = _activePushers[0];
            
            // Tìm ra hướng hộp đang bị đẩy (VD: đang đẩy sang Phải -> X = 1)
            Vector2 pushDirection = new Vector2(Mathf.Sign(_sumPushDirections.x), 0f);
            
            // Hướng dò tìm sẽ là ngược lại về phía sau lưng (VD: sang Trái -> X = -1)
            Vector2 lookBehindDirection = -pushDirection;

            Collider2D col = frontPlayer.GetComponent<Collider2D>();
            if (col != null)
            {
                // Quét một vùng hộp ngay sau lưng người đang đẩy
                RaycastHit2D[] hits = Physics2D.BoxCastAll(
                    col.bounds.center,
                    col.bounds.size * 0.9f, // Thu nhỏ nhẹ để không quẹt trúng trần/sàn nhà
                    0f,
                    lookBehindDirection,
                    1.2f // Khoảng cách quét: Đủ cho 1 nhân vật đứng sát ngay sau lưng
                );

                foreach (var hit in hits)
                {
                    PlayerController backPlayer = hit.collider.GetComponent<PlayerController>();
                    
                    // Nếu phát hiện có Player khác bám ngay sau lưng
                    if (backPlayer != null && backPlayer != frontPlayer)
                    {
                        return true; // Mở khóa ngay lập tức!
                    }
                }
            }
        }

        // Nếu không thỏa mãn cả 2 trường hợp -> Khóa cứng hộp
        return false; 
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null) return;

        Vector2 avgNormal = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            avgNormal += collision.GetContact(i).normal;
        }
        if (collision.contactCount > 0) avgNormal /= collision.contactCount;

        // Chỉ tính là đang đẩy nếu tiếp xúc từ phương ngang (2 bên hông)
        if (Mathf.Abs(avgNormal.x) > 0.5f)
        {
            // Đưa người chơi vào danh sách nếu chưa có
            if (!_activePushers.Contains(player))
            {
                _activePushers.Add(player);
                _sumPushDirections += new Vector2(-avgNormal.x, 0f).normalized;

                if (boxType == BoxType.Ice || boxType == BoxType.Lava)
                {
                    ElementalIdentity id = player.GetComponent<ElementalIdentity>();
                    if (id != null && id.Type == requiredElementForElementBox)
                    {
                        _qualifiedElementPushersCount++;
                    }
                }
            }
        }
    }

    public BoxType Type => boxType;
}