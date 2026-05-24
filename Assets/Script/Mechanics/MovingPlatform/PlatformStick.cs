using UnityEngine;

public class PlatformStick : MonoBehaviour
{
    [SerializeField] private float minUpNormal = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (IsFromAbove(collision))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (collision.collider.transform.parent == transform)
        {
            collision.collider.transform.SetParent(null);
        }
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
