using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableBox : MonoBehaviour
{
    public enum BoxType
    {
        Normal = 0,
        Heavy = 1,
        Ice = 2,
        Lava = 3
    }

    [Header("Type")]
    [SerializeField] private BoxType boxType = BoxType.Normal;

    [Header("Heavy Settings")]
    [SerializeField] private float heavyRequiredDirectionDot = 0.75f;

    [Header("Element Settings")]
    [SerializeField] private ElementalType requiredElementForElementBox = ElementalType.Water;

    private Rigidbody2D _rb;
    private RigidbodyConstraints2D _baseConstraints;

    private int _pushingPlayerCount;
    private int _pushingQualifiedElementCount;

    private Vector2 _sumPushDirections;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _baseConstraints = _rb.constraints;

        if (boxType == BoxType.Ice)
        {
            requiredElementForElementBox = ElementalType.Water;
        }
        else if (boxType == BoxType.Lava)
        {
            requiredElementForElementBox = ElementalType.Fire;
        }
    }

    private void FixedUpdate()
    {
        ApplyConstraintsForType();
        _sumPushDirections = Vector2.zero;
    }

    private void ApplyConstraintsForType()
    {
        RigidbodyConstraints2D constraints = _baseConstraints;

        switch (boxType)
        {
            case BoxType.Normal:
                _rb.constraints = constraints;
                break;

            case BoxType.Heavy:
                bool allowHeavyMove = ShouldAllowHeavyMovement();
                if (!allowHeavyMove)
                {
                    constraints |= RigidbodyConstraints2D.FreezePositionX;
                }
                _rb.constraints = constraints;
                break;

            case BoxType.Ice:
            case BoxType.Lava:
                bool canPush = _pushingQualifiedElementCount > 0;
                if (!canPush)
                {
                    constraints |= RigidbodyConstraints2D.FreezePositionX;
                }
                _rb.constraints = constraints;
                break;
        }
    }

    private bool ShouldAllowHeavyMovement()
    {
        if (_pushingPlayerCount < 2)
        {
            return false;
        }

        Vector2 combined = _sumPushDirections;
        if (combined.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        combined.Normalize();

        return Mathf.Abs(combined.x) >= heavyRequiredDirectionDot;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        Vector2 avgNormal = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            avgNormal += collision.GetContact(i).normal;
        }
        if (collision.contactCount > 0)
        {
            avgNormal /= collision.contactCount;
        }

        Vector2 pushDir = new Vector2(-avgNormal.x, 0f);
        if (pushDir.sqrMagnitude > 0.0001f)
        {
            pushDir.Normalize();
        }

        _sumPushDirections += pushDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        _pushingPlayerCount++;

        if (IsElementRestricted())
        {
            ElementalIdentity id = player.GetComponent<ElementalIdentity>();
            if (id != null && id.Type == requiredElementForElementBox)
            {
                _pushingQualifiedElementCount++;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        _pushingPlayerCount--;
        if (_pushingPlayerCount < 0)
        {
            _pushingPlayerCount = 0;
        }

        if (IsElementRestricted())
        {
            ElementalIdentity id = player.GetComponent<ElementalIdentity>();
            if (id != null && id.Type == requiredElementForElementBox)
            {
                _pushingQualifiedElementCount--;
                if (_pushingQualifiedElementCount < 0)
                {
                    _pushingQualifiedElementCount = 0;
                }
            }
        }
    }

    private bool IsElementRestricted()
    {
        return boxType == BoxType.Ice || boxType == BoxType.Lava;
    }

    public BoxType Type => boxType;
}
