using UnityEngine;
using System.Collections;

public class TrapPlatform : MonoBehaviour
{
    [Header("Settings")]
    public string playerLayerName = "Player";
    public float delayBeforeFall = 3f;
    public float resetDelay = 3f;

    private bool isFalling = false;
    private Vector3 startPos;
    private Coroutine fallCoroutine;

    void Awake()
    {
        startPos = transform.position;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(playerLayerName)) return;
        if (isFalling) return;

        fallCoroutine = StartCoroutine(FallSequence());
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(playerLayerName)) return;
        if (isFalling) return;

        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }
    }

    IEnumerator FallSequence()
    {
        yield return new WaitForSeconds(delayBeforeFall);

        isFalling = true;
        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(resetDelay);
        ResetPlatform();
    }

    void ResetPlatform()
    {
        isFalling = false;
        transform.position = startPos;
        GetComponent<Collider2D>().enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        fallCoroutine = null;
    }
}
