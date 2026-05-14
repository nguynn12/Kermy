using UnityEngine;
using UnityEngine.Events;
// This script should be attached to the Bounce Pad GameObject, which should have a Collider2D (set as trigger) and a Rigidbody2D (set to Kinematic).
public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 14f;
    [SerializeField] private float minUpNormal = 0.5f;

    [SerializeField] private UnityEvent onBounced;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (!IsFromAbove(collision))
        {
            return;
        }

        player.ApplyBounce(bounceForce);
        onBounced?.Invoke();
    }

    private bool IsFromAbove(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 n = collision.GetContact(i).normal;
            if (n.y >= minUpNormal)
            {
                return true;
            }
        }

        return false;
    }
}
