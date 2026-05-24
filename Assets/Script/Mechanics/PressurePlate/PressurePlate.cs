using System;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public event Action<bool> PressedStateChanged;

    [SerializeField] private UnityEvent<bool> pressedStateChanged;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite releasedSprite;
    [SerializeField] private Sprite pressedSprite;

    public bool IsPressed { get; private set; }

    private int _pressingCount;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (releasedSprite == null && spriteRenderer != null)
        {
            releasedSprite = spriteRenderer.sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidPresser(other))
        {
            return;
        }

        _pressingCount++;
        ApplyPressedState(_pressingCount > 0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidPresser(other))
        {
            return;
        }

        _pressingCount--;
        if (_pressingCount < 0)
        {
            _pressingCount = 0;
        }

        ApplyPressedState(_pressingCount > 0);
    }

    private void ApplyPressedState(bool pressed)
    {
        if (IsPressed == pressed)
        {
            return;
        }

        IsPressed = pressed;
        UpdateSprite();
        PressedStateChanged?.Invoke(IsPressed);
        pressedStateChanged?.Invoke(IsPressed);
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite targetSprite = IsPressed ? pressedSprite : releasedSprite;
        if (targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }

    private static bool IsValidPresser(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.GetComponent<PlayerController>() != null)
        {
            return true;
        }

        if (other.attachedRigidbody != null)
        {
            return true;
        }

        return false;
    }
}
