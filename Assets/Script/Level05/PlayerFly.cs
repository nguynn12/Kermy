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
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1)
        {
            return KeybindingManager.GetMoveInputForMap("Player1_Map");
        }

        if (profile == ControlProfile.Player2)
        {
            return KeybindingManager.GetMoveInputForMap("Player2_Map");
        }

        Vector2 movement = Vector2.zero;
        if (Input.GetKey(upKey)) movement.y += 1f;
        if (Input.GetKey(downKey)) movement.y -= 1f;
        if (Input.GetKey(leftKey)) movement.x -= 1f;
        if (Input.GetKey(rightKey)) movement.x += 1f;
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
}
