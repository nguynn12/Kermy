using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private int requiredKeys = 1;
    [SerializeField] private Collider2D solidDoorCollider;
    [SerializeField] private Collider2D exitZoneTrigger;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Transition")]
    [SerializeField] private int requiredPlayersInZone = 2;

    public bool IsOpen { get; private set; }

    private int _playersInZone;
    private bool _transitionTriggered;

    private void Reset()
    {
        solidDoorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        if (solidDoorCollider == null)
        {
            solidDoorCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (closedSprite == null && spriteRenderer != null)
        {
            closedSprite = spriteRenderer.sprite;
        }

        SetOpen(false);
    }

    private void Update()
    {
        if (LevelManager.Instance == null)
        {
            return;
        }

        if (!IsOpen && LevelManager.Instance.CollectedKeys >= requiredKeys)
        {
            SetOpen(true);
        }

        if (!_transitionTriggered && IsOpen && _playersInZone >= requiredPlayersInZone)
        {
            _transitionTriggered = true;
            LevelManager.Instance.LoadNextLevel();
        }
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;

        if (solidDoorCollider != null)
        {
            solidDoorCollider.enabled = !open;
        }

        if (exitZoneTrigger != null)
        {
            exitZoneTrigger.enabled = open;
        }

        if (spriteRenderer != null)
        {
            Sprite targetSprite = open ? openSprite : closedSprite;
            if (targetSprite != null)
            {
                spriteRenderer.sprite = targetSprite;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOpen)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        _playersInZone++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsOpen)
        {
            return;
        }

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
