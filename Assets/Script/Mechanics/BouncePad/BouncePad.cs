using UnityEngine;
using UnityEngine.Events;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 14f;
    [SerializeField] private float bounceCooldown = 0.12f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite bouncedSprite;
    [SerializeField] private float bouncedSpriteDuration = 0.12f;

    [Header("Events")]
    [SerializeField] private UnityEvent onBounced;

    private float _nextBounceTime;
    private float _showReadySpriteAt;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (readySprite == null && spriteRenderer != null)
        {
            readySprite = spriteRenderer.sprite;
        }
    }

    private void Update()
    {
        if (_showReadySpriteAt > 0f && Time.time >= _showReadySpriteAt)
        {
            SetSprite(readySprite);
            _showReadySpriteAt = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryBounce(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryBounce(collision);
    }

    private void TryBounce(Collision2D collision)
    {
        if (Time.time < _nextBounceTime)
        {
            return;
        }

        PlayerController player = collision.collider.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (!IsPlayerAbovePad(player))
        {
            return;
        }

        player.ApplyBounce(bounceForce);
        _nextBounceTime = Time.time + bounceCooldown;

        SetSprite(bouncedSprite);
        _showReadySpriteAt = Time.time + bouncedSpriteDuration;

        onBounced?.Invoke();
    }

    private bool IsPlayerAbovePad(PlayerController player)
    {
        return player.transform.position.y > transform.position.y;
    }

    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
