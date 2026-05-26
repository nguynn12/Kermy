using UnityEngine;

public class Key : MonoBehaviour
{
    public static int FallbackCollectedKeys { get; private set; }

    private bool _collected;

    [SerializeField] private Collider2D keyCollider;
    [SerializeField] private TrailingMovement trailingMovement;

    public PlayerController Holder { get; private set; }

    private void Reset()
    {
        keyCollider = GetComponent<Collider2D>();
        trailingMovement = GetComponent<TrailingMovement>();
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
        if (player == null)
        {
            return;
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

        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        if (trailingMovement != null)
        {
            trailingMovement.SetTarget(player.transform);
            trailingMovement.SetTrailingEnabled(true);
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
}
