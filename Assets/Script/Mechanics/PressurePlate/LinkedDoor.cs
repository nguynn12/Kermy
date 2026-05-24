using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LinkedDoor : MonoBehaviour
{
    private enum PlateActivationMode
    {
        AnyPressed,
        AllPressed
    }

    [Header("Link")]
    [SerializeField] private PressurePlate plate;
    [SerializeField] private List<PressurePlate> additionalPlates = new List<PressurePlate>();
    [SerializeField] private PlateActivationMode activationMode = PlateActivationMode.AllPressed;
    [SerializeField] private bool allowSwitchControl = true;

    [Header("Movement")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 localOffsetWhenOpen = new Vector3(0f, 3f, 0f);
    [SerializeField] private float moveSpeed = 4f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    private Vector3 _closedLocalPos;
    private Vector3 _openLocalPos;
    private bool _shouldBeOpen;
    private bool _switchOpen;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (closedSprite == null && spriteRenderer != null)
        {
            closedSprite = spriteRenderer.sprite;
        }

        _closedLocalPos = targetTransform.localPosition;
        _openLocalPos = _closedLocalPos + localOffsetWhenOpen;
    }

    private void OnEnable()
    {
        foreach (PressurePlate linkedPlate in GetLinkedPlates())
        {
            linkedPlate.PressedStateChanged += OnPlateStateChanged;
        }

        RefreshOpenState();
    }

    private void OnDisable()
    {
        foreach (PressurePlate linkedPlate in GetLinkedPlates())
        {
            linkedPlate.PressedStateChanged -= OnPlateStateChanged;
        }
    }

    private void Update()
    {
        Vector3 target = _shouldBeOpen ? _openLocalPos : _closedLocalPos;
        targetTransform.localPosition = Vector3.Lerp(targetTransform.localPosition, target, moveSpeed * Time.deltaTime);
    }

    private void OnPlateStateChanged(bool pressed)
    {
        RefreshOpenState();
    }

    private void RefreshOpenState()
    {
        List<PressurePlate> linkedPlates = GetLinkedPlates();
        bool shouldOpen = false;

        if (linkedPlates.Count > 0)
        {
            shouldOpen = activationMode == PlateActivationMode.AllPressed
                ? linkedPlates.All(linkedPlate => linkedPlate.IsPressed)
                : linkedPlates.Any(linkedPlate => linkedPlate.IsPressed);
        }

        if (allowSwitchControl && _switchOpen)
        {
            shouldOpen = true;
        }

        SetOpenState(shouldOpen);
    }

    public void SetSwitchOpen(bool open)
    {
        if (!allowSwitchControl)
        {
            return;
        }

        _switchOpen = open;
        RefreshOpenState();
    }

    private void SetOpenState(bool open)
    {
        _shouldBeOpen = open;

        if (spriteRenderer == null)
        {
            return;
        }

        Sprite targetSprite = open ? openSprite : closedSprite;
        if (targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
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
}
