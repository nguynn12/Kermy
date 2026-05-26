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
    private readonly System.Collections.Generic.HashSet<PlayerController> _enteredPlayers = new System.Collections.Generic.HashSet<PlayerController>();

    private void Reset()
    {
        solidDoorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        EnsureLevelManagerExists();

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
        if (!IsOpen && GetCollectedKeys() >= requiredKeys)
        {
            SetOpen(true);
        }

        if (!_transitionTriggered && IsOpen && _playersInZone >= requiredPlayersInZone && LevelManager.Instance != null)
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

    private static int GetCollectedKeys()
    {
        return LevelManager.Instance != null
            ? LevelManager.Instance.CollectedKeys
            : Key.FallbackCollectedKeys;
    }

    private static void EnsureLevelManagerExists()
    {
        if (LevelManager.Instance != null)
        {
            return;
        }

        if (FindFirstObjectByType<LevelManager>() != null)
        {
            return;
        }

        new GameObject("LevelManager").AddComponent<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOpen)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || _enteredPlayers.Contains(player))
        {
            return;
        }

        _enteredPlayers.Add(player);
        _playersInZone++;
        HideEnteredPlayer(player);

        if (!_transitionTriggered && _playersInZone >= requiredPlayersInZone && LevelManager.Instance != null)
        {
            _transitionTriggered = true;
            LevelManager.Instance.LoadNextLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsOpen)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || _enteredPlayers.Contains(player))
        {
            return;
        }

        _playersInZone--;
        if (_playersInZone < 0)
        {
            _playersInZone = 0;
        }
    }

    private static void HideEnteredPlayer(PlayerController player)
    {
        player.SetControlEnabled(false);

        foreach (SpriteRenderer renderer in player.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }

        foreach (Collider2D playerCollider in player.GetComponentsInChildren<Collider2D>())
        {
            playerCollider.enabled = false;
        }

        foreach (Key key in FindObjectsByType<Key>(FindObjectsSortMode.None))
        {
            key.HideIfHeldBy(player);
        }

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.linearVelocity = Vector2.zero;
            playerBody.simulated = false;
        }
    }
}
