using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float minUpNormal = 0.5f;

    private Rigidbody2D _rb;
    private bool _armed;
    private Coroutine _routine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_armed)
        {
            return;
        }

        if (_rb == null || _rb.bodyType != RigidbodyType2D.Kinematic)
        {
            return;
        }

        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (!IsFromAbove(collision))
        {
            return;
        }

        _armed = true;
        _routine = StartCoroutine(FallAfterDelay());
    }

    private IEnumerator FallAfterDelay()
    {
        float delay = fallDelay;
        if (delay < 0f)
        {
            delay = 0f;
        }

        yield return new WaitForSeconds(delay);

        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
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
