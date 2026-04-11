using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public event Action<bool> PressedStateChanged;

    public bool IsPressed { get; private set; }

    private int _pressingCount;

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
        PressedStateChanged?.Invoke(IsPressed);
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
