using UnityEngine;

public class Key : MonoBehaviour
{
    public static int FallbackCollectedKeys { get; private set; }

    [Header("Door")]
    [SerializeField] private ExitDoor targetDoor;
    [SerializeField] private bool requireMatchingElement;
    [SerializeField] private ElementalType requiredElement;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("Floating")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.15f;

    [Header("Follow Holder")]
    [SerializeField] private Collider2D keyCollider;
    [SerializeField] private TrailingMovement trailingMovement;

    private bool _collected;
    private Vector3 _startPos;

    public PlayerController Holder { get; private set; }

    private void Reset()
    {
        keyCollider = GetComponent<Collider2D>();
        trailingMovement = GetComponent<TrailingMovement>();
    }

    private void Awake()
    {
        _startPos = transform.position;

        if (keyCollider == null)
        {
            keyCollider = GetComponent<Collider2D>();
        }

        if (trailingMovement == null)
        {
            trailingMovement = GetComponent<TrailingMovement>();
        }
    }

    private void Update()
    {
        if (_collected || trailingMovement != null || floatHeight <= 0f)
        {
            return;
        }

        float y = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetFallbackKeys()
    {
        FallbackCollectedKeys = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || !CanPlayerCollect(other))
        {
            return;
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterKeyCollected();
        }
        else
        {
            FallbackCollectedKeys++;
        }

        _collected = true;
        Holder = player;

        if (targetDoor != null)
        {
            targetDoor.Unlock();
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        if (trailingMovement != null)
        {
            trailingMovement.SetTarget(player.transform);
            trailingMovement.SetTrailingEnabled(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void HideIfHeldBy(PlayerController player)
    {
        if (Holder != player)
        {
            return;
        }

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }

        if (trailingMovement != null)
        {
            trailingMovement.SetTrailingEnabled(false);
        }
    }

    private bool CanPlayerCollect(Collider2D other)
    {
        if (!requireMatchingElement && targetDoor == null)
        {
            return true;
        }

        ElementalIdentity identity = other.GetComponent<ElementalIdentity>();
        return identity != null && identity.Type == requiredElement;
    }
}
