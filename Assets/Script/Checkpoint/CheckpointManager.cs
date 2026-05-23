using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPos;
    private bool hasCheckpoint = false;

    // ====================================================================
    // 🔥 KHU VỰC AUDIO KHÔNG SỬA LOGIC CŨ: Chỉ thêm ô để Mạnh kéo file Boom.wav
    // ====================================================================
    [Header("Audio Settings")]
    public AudioSource audioSource;   // Cái loa (Mạnh gắn loa lên chính cái CheckpointManager này luôn)
    public AudioClip checkpointSound; // Ô để thả file Boom.wav vào

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tự động tìm cái loa gắn chung trên Object này
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        // Kiểm tra nếu đây là lần đầu tiên chạm vào Checkpoint này (hoặc vị trí mới) thì mới phát tiếng nổ
        // Điều này giúp tránh việc con ếch đứng dậm chân tại chỗ ở trụ mà tiếng cứ "Boom" liên tục chói tai
        if (lastCheckpointPos != position || !hasCheckpoint)
        {
            // 🔥 KÍCH HOẠT TIẾNG BOOM KHI CHẠM TRỤ THÀNH CÔNG!
            if (audioSource != null && checkpointSound != null)
            {
                audioSource.PlayOneShot(checkpointSound, 0.8f); // Âm lượng 80% cho hoành tráng
            }
        }

        lastCheckpointPos = position;
        hasCheckpoint = true;
    }

    // ====================================================================
    // 🔥 HÀM PHÉP THUẬT MỚI: Vừa dịch chuyển vừa chống dính lẹo cho 2 nhân vật (GIỮ NGUYÊN 100%)
    // ====================================================================
    public void RespawnPlayers(GameObject player1, GameObject player2, Vector3 defaultSpawn)
    {
        Vector3 spawnTarget = hasCheckpoint ? lastCheckpointPos : defaultSpawn;

        if (player1 == null || player2 == null) return;

        player1.transform.position = spawnTarget;
        player2.transform.position = spawnTarget + new Vector3(1f, 0f, 0f);

        Collider2D c1 = player1.GetComponent<Collider2D>();
        Collider2D c2 = player2.GetComponent<Collider2D>();

        if (c1 != null && c2 != null)
        {
            Physics2D.IgnoreCollision(c1, c2, true);
            StartCoroutine(EnableCollisionAfterDelay(c1, c2, 0.5f));
        }
    }

    private System.Collections.IEnumerator EnableCollisionAfterDelay(Collider2D c1, Collider2D c2, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (c1 != null && c2 != null)
        {
            Physics2D.IgnoreCollision(c1, c2, false);
        }
    }

    public Vector3 GetRespawnPosition(Vector3 defaultSpawn)
    {
        return hasCheckpoint ? lastCheckpointPos : defaultSpawn;
    }

    public void ResetAll()
    {
        hasCheckpoint = false;
    }
}