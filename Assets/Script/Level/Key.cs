using UnityEngine;

public class Key : MonoBehaviour
{
    private bool _collected;

    [SerializeField] private Collider2D keyCollider;
    [SerializeField] private TrailingMovement trailingMovement;

    private void Reset()
    {
        keyCollider = GetComponent<Collider2D>();
        trailingMovement = GetComponent<TrailingMovement>();
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

        _collected = true;

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
}
