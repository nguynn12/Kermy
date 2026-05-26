<<<<<<< HEAD
﻿using System.Collections.Generic;
=======
using System.Collections.Generic;
using System;
>>>>>>> origin/level3
using UnityEngine;

public class Lever : MonoBehaviour
{
    [System.Serializable]
    private class DoorTarget
    {
        public LinkedDoor door;
        public bool openWhenOn = true;
    }

    [System.Serializable]
    private class LaserTarget
    {
        public LaserBarrier laser;
        public bool onWhenLeverOn;
    }

    [Header("State")]
    [SerializeField] private bool startOn;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    [Header("Targets")]
    [SerializeField] private List<DoorTarget> doors = new List<DoorTarget>();
    [SerializeField] private List<LaserTarget> lasers = new List<LaserTarget>();

    public bool IsOn { get; private set; }
<<<<<<< HEAD
=======
    public event Action<bool> StateChanged;
>>>>>>> origin/level3

    private readonly List<PlayerInputHandler> _playersInRange = new List<PlayerInputHandler>();

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Collider2D leverCollider = GetComponent<Collider2D>();
        if (leverCollider != null)
        {
            leverCollider.isTrigger = true;
        }
    }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (offSprite == null && spriteRenderer != null)
        {
            offSprite = spriteRenderer.sprite;
        }

        SetState(startOn);
    }

<<<<<<< HEAD
=======
    private void Start()
    {
        SetState(startOn);
    }

>>>>>>> origin/level3
    private void Update()
    {
        for (int i = _playersInRange.Count - 1; i >= 0; i--)
        {
            PlayerInputHandler input = _playersInRange[i];
            if (input == null)
            {
                _playersInRange.RemoveAt(i);
                continue;
            }

            if (input.ConsumeActionPressed())
            {
                Toggle();
                return;
            }
        }
    }

    public void Toggle()
    {
        SetState(!IsOn);
    }

    public void SetState(bool on)
    {
<<<<<<< HEAD
        IsOn = on;
        UpdateSprite();
        ApplyTargets();
=======
        bool changed = IsOn != on;
        IsOn = on;
        UpdateSprite();
        ApplyTargets();

        if (changed)
        {
            StateChanged?.Invoke(IsOn);
        }
>>>>>>> origin/level3
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite targetSprite = IsOn ? onSprite : offSprite;
        if (targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }

    private void ApplyTargets()
    {
        foreach (DoorTarget target in doors)
        {
            if (target.door != null)
            {
<<<<<<< HEAD
                // Tạm thời gọi hàm SetOpen thay vì SetSwitchOpen
                // Hãy kiểm tra lại trong LinkedDoor.cs tên hàm đúng là gì
                //target.door.SetOpen(IsOn == target.openWhenOn);
=======
                target.door.SetSwitchOpen(IsOn == target.openWhenOn);
>>>>>>> origin/level3
            }
        }

        foreach (LaserTarget target in lasers)
        {
            if (target.laser != null)
            {
<<<<<<< HEAD
                target.laser.SetState(IsOn == target.onWhenLeverOn);
=======
                target.laser.SetSwitchOn(IsOn == target.onWhenLeverOn);
>>>>>>> origin/level3
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInputHandler input = other.GetComponentInParent<PlayerInputHandler>();
        if (input != null && !_playersInRange.Contains(input))
        {
<<<<<<< HEAD
=======
            input.ClearActionPressed();
>>>>>>> origin/level3
            _playersInRange.Add(input);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInputHandler input = other.GetComponentInParent<PlayerInputHandler>();
        if (input != null)
        {
            _playersInRange.Remove(input);
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> origin/level3
