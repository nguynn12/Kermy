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

    // ====================================================================
    // 🔥 HÀM PHÉP THUẬT MỚI: Vừa dịch chuyển vừa chống dính lẹo cho 2 nhân vật
    // ====================================================================
    public void RespawnPlayers(GameObject player1, GameObject player2, Vector3 defaultSpawn)
    {
        // 1. Lấy vị trí hồi sinh chuẩn (Checkpoint hoặc điểm xuất phát mặc định)
        Vector3 spawnTarget = hasCheckpoint ? lastCheckpointPos : defaultSpawn;

        if (player1 == null || player2 == null) return;

        // 2. Dịch chuyển 2 con ếch về vị trí (Cho con Nước lệch sang phải 1 mét cho đẹp đội hình)
        player1.transform.position = spawnTarget;
        player2.transform.position = spawnTarget + new Vector3(1f, 0f, 0f);

        // 3. Lấy Collider của 2 đứa để xử lý xuyên thấu tạm thời
        Collider2D c1 = player1.GetComponent<Collider2D>();
        Collider2D c2 = player2.GetComponent<Collider2D>();

        if (c1 != null && c2 != null)
        {
            // Ép 2 cái Collider này đi xuyên qua nhau ngay lập tức để không bị kẹt dính
            Physics2D.IgnoreCollision(c1, c2, true);

            // Tạo một Object tạm thời để chạy giờ hẹn (vì IgnoreCollision cần tắt sau 0.5s)
            StartCoroutine(EnableCollisionAfterDelay(c1, c2, 0.5f));
        }
    }

    // Hàm đếm ngược thời gian bằng Coroutine của Unity
    private System.Collections.IEnumerator EnableCollisionAfterDelay(Collider2D c1, Collider2D c2, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Sau 0.5 giây, nếu 2 con ếch vẫn còn sống thì bật lại va chạm để tha hồ nhảy lên đầu nhau!
        if (c1 != null && c2 != null)
        {
            Physics2D.IgnoreCollision(c1, c2, false);
        }
    }

    // Hàm cũ (Giữ lại để nếu code khác có gọi thì không bị lỗi)
    public Vector3 GetRespawnPosition(Vector3 defaultSpawn)
    {
        return hasCheckpoint ? lastCheckpointPos : defaultSpawn;
    }

    public void ResetAll()
    {
        hasCheckpoint = false;
    }
}