using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Vector3 spawnPosition;

    private void Start()
    {
        // Lưu lại vị trí bắt đầu phòng trường hợp người chơi chưa chạm vào hòn đá nào đã chết
        spawnPosition = transform.position;
    }

    public void Kill()
    {
        // 1. Lấy vị trí hồi sinh chuẩn (Hỏi CheckpointManager, nếu không có thì lấy spawnPosition mặc định)
        Vector3 respawnTarget = spawnPosition;
        if (CheckpointManager.Instance != null)
        {
            respawnTarget = CheckpointManager.Instance.GetRespawnPosition(spawnPosition);
        }

        // 2. TÌM TẤT CẢ CÁC ĐỨA CÓ SCRIPT PLAYERHEALTH TRÊN MAP (Gồm cả Ếch Lửa và Ếch Nước)
        PlayerHealth[] allPlayers = Object.FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

        // 3. DÙNG VÒNG LẶP ĐỂ BỐC CẢ 2 ĐỨA VỀ CHECKPOINT CÙNG NHAU
        foreach (PlayerHealth player in allPlayers)
        {
            if (player != null)
            {
                // Dịch chuyển nhân vật về hòn đá Checkpoint
                player.transform.position = respawnTarget;

                // DỌN DẸP VẬT LÝ: Triệt tiêu ngay vận tốc để không đứa nào bị quán tính rơi tiếp hay giật lag
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                // KHÓA LẠI CAMERA: Ép camera con của từng đứa phải dịch chuyển theo chuẩn cự ly (Z = -10)
                Camera childCamera = player.GetComponentInChildren<Camera>();
                if (childCamera != null)
                {
                    childCamera.transform.localPosition = new Vector3(0, 0, -10);
                }

                Debug.Log(player.gameObject.name + " đã dắt tay đồng đội hồi sinh tại Checkpoint!");
            }
        }
    }
}