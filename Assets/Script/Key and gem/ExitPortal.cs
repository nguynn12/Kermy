using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public enum PortalType { RedPortal, BluePortal }

    [Header("Cấu hình Cổng")]
    public PortalType loaiCong;

    [Header("Màn chơi tiếp theo")]
    [SerializeField] private Object nextScene; // Kéo thả file Scene Level01 vào đây ngoài Inspector

    private string nextSceneName
    {
        get { return nextScene != null ? nextScene.name : "Level01"; }
    }

    [Header("Âm thanh Chiến Thắng")]
    [SerializeField] private AudioClip victorySound;

    public static bool hasRedKey = false;
    public static bool hasBlueKey = false;

    private static bool p1Win = false;
    private static bool p2Win = false;
    private static bool hasPlayedVictorySound = false;

    private void Start()
    {
        hasRedKey = false;
        hasBlueKey = false;
        p1Win = false;
        p2Win = false;
        hasPlayedVictorySound = false;
    }

    public void CollectKey(PlayerInventory.ElementType element)
    {
        if (element == PlayerInventory.ElementType.Fire)
        {
            hasRedKey = true;
            Debug.Log("Cổng nhận tín hiệu: Đã lấy được Chìa Khóa Lửa!");
        }
        else if (element == PlayerInventory.ElementType.Water)
        {
            hasBlueKey = true;
            Debug.Log("Cổng nhận tín hiệu: Đã lấy được Chìa Khóa Nước!");
        }

        CheckVictory();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ĐÃ SỬA: Check đúng Tag "Player 1" ngoài Inspector của Mạnh
        if (loaiCong == PortalType.RedPortal && other.CompareTag("Player 1"))
        {
            p1Win = true;
            Debug.Log("Ếch Lửa đã đứng vào Cổng Đỏ!");
            CheckVictory();
        }
        // ĐÃ SỬA: Check đúng Tag "Player 2" ngoài Inspector của Mạnh
        else if (loaiCong == PortalType.BluePortal && other.CompareTag("Player 2"))
        {
            p2Win = true;
            Debug.Log("Ếch Nước đã đứng vào Cổng Xanh!");
            CheckVictory();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (loaiCong == PortalType.RedPortal && other.CompareTag("Player 1")) p1Win = false;
        else if (loaiCong == PortalType.BluePortal && other.CompareTag("Player 2")) p2Win = false;
    }

    private void CheckVictory()
    {
        if (p1Win && p2Win)
        {
            if (hasRedKey && hasBlueKey)
            {
                Debug.Log("XUẤT SẮC! Đủ 2 chìa khóa và cả hai đã qua màn!");

                if (victorySound != null && !hasPlayedVictorySound)
                {
                    hasPlayedVictorySound = true;
                    AudioSource.PlayClipAtPoint(victorySound, transform.position, 0.9f);
                }
                // Bỏ qua hoàn toàn bảng chiến thắng lỗi - Chuyển màn luôn sau 0.5 giây
                Invoke("LoadNextScene", 0.5f);
            }
            else
            {
                Debug.Log("Cả hai về đúng vị trí nhưng vẫn THIẾU CHÌA KHÓA!");
            }
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
