using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

public class LaserBarrier : MonoBehaviour
{
    private enum PlateActivationMode
    {
        AnyPressed,
        AllPressed
    }

    [SerializeField] private bool startOn = true;
    [SerializeField] private Collider2D barrierCollider;

    [Header("Plate Controls")]
    [SerializeField] private PressurePlate plate;
    [SerializeField] private List<PressurePlate> additionalPlates = new List<PressurePlate>();
    [SerializeField] private PlateActivationMode plateActivationMode = PlateActivationMode.AnyPressed;
    [SerializeField] private bool laserOnWhenPlatesPressed;

    [Header("Lever/Switch Controls")]
    [SerializeField] private bool allowSwitchControl = true;

    [Header("Beam")]
    [FormerlySerializedAs("spriteRenderer")]
    [SerializeField] private SpriteRenderer beamRenderer;

    [Header("Laser Heads")]
    [SerializeField] private SpriteRenderer emitterRenderer;
    [SerializeField] private Sprite emitterOnSprite;
    [SerializeField] private Sprite emitterOffSprite;
    [SerializeField] private SpriteRenderer receiverRenderer;
    [SerializeField] private Sprite receiverOnSprite;
    [SerializeField] private Sprite receiverOffSprite;

    public bool IsOn { get; private set; }

    private bool _switchHasState;
    private bool _switchOn;

    private void Reset()
    {
        barrierCollider = GetComponent<Collider2D>();
        beamRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        foreach (PressurePlate linkedPlate in GetLinkedPlates())
        {
            linkedPlate.PressedStateChanged += OnPlateStateChanged;
        }

        RefreshState();
    }

    private void OnDisable()
    {
        foreach (PressurePlate linkedPlate in GetLinkedPlates())
        {
            linkedPlate.PressedStateChanged -= OnPlateStateChanged;
        }
    }

    private void Awake()
    {
        RefreshState();
    }

    public void SetSwitchOn(bool on)
    {
        if (!allowSwitchControl)
        {
            return;
        }

        _switchHasState = true;
        _switchOn = on;
        RefreshState();
    }

    public void SetState(bool on)
    {
        SetSwitchOn(on);
    }

    private void OnPlateStateChanged(bool pressed)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        bool shouldBeOn = startOn;
        bool hasControlSource = false;

        List<PressurePlate> linkedPlates = GetLinkedPlates();
        if (linkedPlates.Count > 0)
        {
            hasControlSource = true;
            bool plateConditionMet = plateActivationMode == PlateActivationMode.AllPressed
                ? linkedPlates.All(linkedPlate => linkedPlate.IsPressed)
                : linkedPlates.Any(linkedPlate => linkedPlate.IsPressed);

            shouldBeOn = plateConditionMet ? laserOnWhenPlatesPressed : !laserOnWhenPlatesPressed;
        }

        if (allowSwitchControl && _switchHasState)
        {
            shouldBeOn = hasControlSource ? shouldBeOn && _switchOn : _switchOn;
            hasControlSource = true;
        }

        ApplyState(shouldBeOn);
    }

    private void ApplyState(bool on)
    {
        IsOn = on;

        if (barrierCollider != null)
        {
            barrierCollider.enabled = on;
        }

        if (beamRenderer != null)
        {
            beamRenderer.enabled = on;
            beamRenderer.color = Color.white;
        }

        SetHeadSprite(emitterRenderer, on ? emitterOnSprite : emitterOffSprite);
        SetHeadSprite(receiverRenderer, on ? receiverOnSprite : receiverOffSprite);
    }

    private List<PressurePlate> GetLinkedPlates()
    {
        List<PressurePlate> linkedPlates = new List<PressurePlate>();

        if (plate != null)
        {
            linkedPlates.Add(plate);
        }

        foreach (PressurePlate linkedPlate in additionalPlates)
        {
            if (linkedPlate != null && !linkedPlates.Contains(linkedPlate))
            {
                linkedPlates.Add(linkedPlate);
            }
        }

        return linkedPlates;
    }

    private static void SetHeadSprite(SpriteRenderer renderer, Sprite sprite)
    {
        if (renderer != null && sprite != null)
        {
            renderer.sprite = sprite;
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
