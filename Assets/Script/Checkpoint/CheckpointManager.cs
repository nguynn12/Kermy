using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPos;
    private bool hasCheckpoint = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPos = position;
        hasCheckpoint = true;
    }

    // Gọi khi nhân vật chết để respawn
    public Vector3 GetRespawnPosition(Vector3 defaultSpawn)
    {
        return hasCheckpoint ? lastCheckpointPos : defaultSpawn;
    }

    public void ResetAll()
    {
        hasCheckpoint = false;
    }
}
