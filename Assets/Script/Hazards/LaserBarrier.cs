using UnityEngine;

public class LaserBarrier : MonoBehaviour
{
    [SerializeField] private bool startOn = true;
    [SerializeField] private Collider2D barrierCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Visual")]
    [SerializeField] private Color onColor = Color.red;
    [SerializeField] private Color offColor = new Color(1f, 0f, 0f, 0.2f);

    public bool IsOn { get; private set; }

    private void Reset()
    {
        barrierCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        SetState(startOn);
    }

    public void SetState(bool on)
    {
        IsOn = on;

        if (barrierCollider != null)
        {
            barrierCollider.enabled = on;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = on ? onColor : offColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOn)
        {
            return;
        }

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Kill();
        }
    }
}
