using UnityEngine;

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

    private int _sidePushersCount;
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
        // Reset biến đếm mỗi khung hình vật lý
        _sidePushersCount = 0;
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
        if (_sidePushersCount < 2) return false;
        Vector2 combined = _sumPushDirections.normalized;
        return Mathf.Abs(combined.x) >= heavyRequiredDirectionDot;
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
            _sidePushersCount++;
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

    public BoxType Type => boxType;
}