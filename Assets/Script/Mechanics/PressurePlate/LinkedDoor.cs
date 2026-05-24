using UnityEngine;

public class LinkedDoor : MonoBehaviour
{
    [Header("Link")]
    [SerializeField] private PressurePlate plate;

    [Header("Movement")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 localOffsetWhenOpen = new Vector3(0f, 3f, 0f);
    [SerializeField] private float moveSpeed = 4f;

    private Vector3 _closedLocalPos;
    private Vector3 _openLocalPos;
    private bool _shouldBeOpen;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        _closedLocalPos = targetTransform.localPosition;
        _openLocalPos = _closedLocalPos + localOffsetWhenOpen;
    }

    private void OnEnable()
    {
        if (plate != null)
        {
            plate.PressedStateChanged += OnPlateStateChanged;
            OnPlateStateChanged(plate.IsPressed);
        }
    }

    private void OnDisable()
    {
        if (plate != null)
        {
            plate.PressedStateChanged -= OnPlateStateChanged;
        }
    }

    private void Update()
    {
        Vector3 target = _shouldBeOpen ? _openLocalPos : _closedLocalPos;
        targetTransform.localPosition = Vector3.Lerp(targetTransform.localPosition, target, moveSpeed * Time.deltaTime);
    }

    private void OnPlateStateChanged(bool pressed)
    {
        _shouldBeOpen = pressed;
    }
}
