using UnityEngine;

public class TrailingMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 localOffset = new Vector3(-0.6f, 0.4f, 0f);
    [SerializeField] private float smoothTime = 0.12f;

    [Header("Wiggle")]
    [SerializeField] private float bobAmplitude = 0.12f;
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float rotationAmplitude = 10f;

    private bool _enabled;
    private Vector3 _velocity;
    private Vector3 _baseOffset;

    private void Awake()
    {
        _baseOffset = localOffset;
    }

    private void Update()
    {
        if (!_enabled || target == null)
        {
            return;
        }

        Vector3 desiredOffset = _baseOffset;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        desiredOffset.y += bob;

        Vector3 desiredPos = target.position + desiredOffset;
        desiredPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _velocity, smoothTime);

        float rotZ = Mathf.Sin(Time.time * bobFrequency) * rotationAmplitude;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetTrailingEnabled(bool enabled)
    {
        _enabled = enabled;
        if (!enabled)
        {
            _velocity = Vector3.zero;
        }
    }
}
