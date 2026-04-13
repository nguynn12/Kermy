using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformRespawn : MonoBehaviour
{
    [SerializeField] private float respawnBelowY = -20f;

    private Rigidbody2D _rb;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (transform.position.y >= respawnBelowY)
        {
            return;
        }

        Respawn();
    }

    private void Respawn()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Báo hiệu cho các component khác trên cùng object biết nó vừa được reset
        SendMessage("OnPlatformRespawn", SendMessageOptions.DontRequireReceiver);
    }
}
