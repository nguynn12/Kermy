using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private int requiredKeys = 1;

    [Header("Colliders")]
    [SerializeField] private Collider2D solidDoorCollider;
    [SerializeField] private Collider2D exitZoneTrigger;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite spriteClosed;
    [SerializeField] private Sprite spriteOpen;

    [Header("Transition")]
    [SerializeField] private int requiredPlayersInZone = 2;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float loadDelay = 1f;

    [Header("Element Gate")]
    [SerializeField] private bool requireMatchingElement;
    [SerializeField] private ElementalType requiredElement;

    [Header("Audio")]
    [SerializeField] private AudioClip soundOpen;
    [SerializeField] private AudioClip soundEnter;

    public bool IsOpen { get; private set; }

    private AudioSource _audioSource;
    private bool _isUnlocked;
    private bool _transitionTriggered;
    private readonly HashSet<PlayerController> _enteredPlayers = new HashSet<PlayerController>();

    private void Reset()
    {
        solidDoorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
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

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (closedSprite == null)
        {
            closedSprite = spriteClosed != null ? spriteClosed : spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        if (openSprite == null)
        {
            openSprite = spriteOpen;
        }

        SetOpen(false);
    }

    private void Update()
    {
        if (!_isUnlocked && GetCollectedKeys() >= requiredKeys)
        {
            Unlock();
        }

        TryLoadNextLevel();
    }

    public void Unlock()
    {
        if (_isUnlocked)
        {
            return;
        }

        _isUnlocked = true;
        SetOpen(true);
        PlaySound(soundOpen);
        TryLoadNextLevel();
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

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || _enteredPlayers.Contains(player) || !CanPlayerEnter(other))
        {
            return;
        }

        _enteredPlayers.Add(player);
        HideEnteredPlayer(player);
        PlaySound(soundEnter);
        TryLoadNextLevel();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || !_enteredPlayers.Contains(player))
        {
            return;
        }

        _enteredPlayers.Remove(player);
    }

    private bool CanPlayerEnter(Collider2D other)
    {
        if (!requireMatchingElement && requiredPlayersInZone > 1)
        {
            return true;
        }

        ElementalIdentity identity = other.GetComponent<ElementalIdentity>();
        return identity == null || identity.Type == requiredElement;
    }

    private void TryLoadNextLevel()
    {
        if (_transitionTriggered || !IsOpen || _enteredPlayers.Count < Mathf.Max(1, requiredPlayersInZone))
        {
            return;
        }

        _transitionTriggered = true;
        StartCoroutine(LoadNextLevelAfterDelay());
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        if (loadDelay > 0f)
        {
            yield return new WaitForSeconds(loadDelay);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
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

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
