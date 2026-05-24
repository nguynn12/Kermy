using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private int requiredKeys = 1;
    [SerializeField] private Collider2D doorCollider;

    [Header("Transition")]
    [SerializeField] private int requiredPlayersInZone = 2;

    public bool IsOpen { get; private set; }

    private int _playersInZone;
    private bool _transitionTriggered;

    private void Reset()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (LevelManager.Instance == null)
        {
            return;
        }

        if (!IsOpen && LevelManager.Instance.CollectedKeys >= requiredKeys)
        {
            Open();
        }

        if (!_transitionTriggered && IsOpen && _playersInZone >= requiredPlayersInZone)
        {
            _transitionTriggered = true;
            LevelManager.Instance.LoadNextLevel();
        }
    }

    private void Open()
    {
        IsOpen = true;
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        _playersInZone++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        _playersInZone--;
        if (_playersInZone < 0)
        {
            _playersInZone = 0;
        }
    }
}
