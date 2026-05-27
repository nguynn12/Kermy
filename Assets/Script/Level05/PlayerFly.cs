using UnityEngine;

public class PlayerFly : MonoBehaviour
{
    private enum ControlProfile
    {
        Auto,
        Player1,
        Player2,
        Custom
    }

    [Header("Flight")]
    [SerializeField] private ControlProfile controlProfile = ControlProfile.Auto;
    public float flySpeed = 5f;

    [Header("Custom Keys")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("Screen Bounds")]
    public float minX = -8.5f;
    public float maxX = 8.5f;
    public float minY = -4.5f;
    public float maxY = 4.5f;

    private Rigidbody2D _body;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        if (_body != null)
        {
            _body.gravityScale = 0f;
        }
    }

    private void Update()
    {
        Vector2 movement = ReadMovement().normalized;
        Vector3 nextPosition = transform.position + (Vector3)(movement * flySpeed * Time.deltaTime);
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);

        if (_body != null)
        {
            _body.linearVelocity = movement * flySpeed;
            _body.position = nextPosition;
        }
        else
        {
            transform.position = nextPosition;
        }
    }

    private void OnDisable()
    {
        if (_body != null)
        {
            _body.linearVelocity = Vector2.zero;
        }
    }

    private Vector2 ReadMovement()
    {
        KeyCode up = ResolveUpKey();
        KeyCode down = ResolveDownKey();
        KeyCode left = ResolveLeftKey();
        KeyCode right = ResolveRightKey();

        Vector2 movement = Vector2.zero;
        if (Input.GetKey(up)) movement.y += 1f;
        if (Input.GetKey(down)) movement.y -= 1f;
        if (Input.GetKey(left)) movement.x -= 1f;
        if (Input.GetKey(right)) movement.x += 1f;
        return movement;
    }

    private ControlProfile ResolveProfile()
    {
        if (controlProfile != ControlProfile.Auto)
        {
            return controlProfile;
        }

        string normalizedName = gameObject.name.Replace(" ", string.Empty).ToLowerInvariant();
        if (normalizedName.Contains("player2") || normalizedName.Contains("aqua"))
        {
            return ControlProfile.Player2;
        }

        return ControlProfile.Player1;
    }

    private KeyCode ResolveUpKey()
    {
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1) return KeyCode.W;
        if (profile == ControlProfile.Player2) return KeyCode.UpArrow;
        return upKey;
    }

    private KeyCode ResolveDownKey()
    {
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1) return KeyCode.S;
        if (profile == ControlProfile.Player2) return KeyCode.DownArrow;
        return downKey;
    }

    private KeyCode ResolveLeftKey()
    {
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1) return KeyCode.A;
        if (profile == ControlProfile.Player2) return KeyCode.LeftArrow;
        return leftKey;
    }

    private KeyCode ResolveRightKey()
    {
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1) return KeyCode.D;
        if (profile == ControlProfile.Player2) return KeyCode.RightArrow;
        return rightKey;
    }
}
