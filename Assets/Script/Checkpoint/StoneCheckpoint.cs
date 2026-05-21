using UnityEngine;

public class StoneCheckpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spriteInactive;
    public Sprite spriteActive;

    [Header("Settings")]
    public string playerLayerName = "Player";

    private SpriteRenderer sr;
    private bool isActivated = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteInactive;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
        if (other.gameObject.layer != LayerMask.NameToLayer(playerLayerName)) return;

        Activate();
    }

    void Activate()
    {
        isActivated = true;
        sr.sprite = spriteActive;

        CheckpointManager.Instance?.SetCheckpoint(transform.position);

        Debug.Log($"Checkpoint activated: {gameObject.name}");
    }

    public void Reset()
    {
        isActivated = false;
        sr.sprite = spriteInactive;
    }
}
