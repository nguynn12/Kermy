using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public enum PortalType { RedPortal, BluePortal }

    [Header("Cấu hình Cổng")]
    public PortalType loaiCong; 

    [Header("Màn chơi tiếp theo")]
    [SerializeField] private string nextSceneName = "MenuGame";

    [Header("Âm thanh Chiến Thắng")]
    [SerializeField] private AudioClip victorySound; 

    // ====================================================================
    // 🌟 KHAI BÁO BIẾN TOÀN CỤC: Để sửa lỗi CS0103 cho Mạnh nè!
    // ====================================================================
    public static bool hasRedKey = false;
    public static bool hasBlueKey = false;
    
    private static bool p1Win = false;
    private static bool p2Win = false;
    private static bool hasPlayedVictorySound = false; 

    private void Start()
    {
        // Vào game reset sạch trạng thái về ban đầu
        hasRedKey = false;
        hasBlueKey = false;
        p1Win = false;
        p2Win = false;
        hasPlayedVictorySound = false; 
    }

    // Hàm nhận tín hiệu nhặt chìa khóa từ file KeyItem của bạn
    public void CollectKey(PlayerInventory.ElementType element)
    {
        // Tùy vào hệ của chìa khóa nhặt được mà kích hoạt trạng thái tương ứng
        if (element == PlayerInventory.ElementType.Fire) // Đổi chữ Fire nếu nhóm bạn đặt tên khác
        {
            hasRedKey = true;
            Debug.Log("Cổng nhận tín hiệu: Đã lấy được Chìa Khóa Lửa!");
        }
        else if (element == PlayerInventory.ElementType.Water) // Đổi chữ Water nếu nhóm bạn đặt tên khác
        {
            hasBlueKey = true;
            Debug.Log("Cổng nhận tín hiệu: Đã lấy được Chìa Khóa Nước!");
        }
        
        // Sau khi nhặt, tự động check thử xem 2 đứa đang đứng đợi sẵn ở cổng chưa
        CheckVictory();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (loaiCong == PortalType.RedPortal && other.CompareTag("Player1"))
        {
            p1Win = true;
            Debug.Log("Ếch Lửa đã đứng vào Cổng Đỏ!");
            CheckVictory();
        }
        else if (loaiCong == PortalType.BluePortal && other.CompareTag("Player2"))
        {
            p2Win = true;
            Debug.Log("Ếch Nước đã đứng vào Cổng Xanh!");
            CheckVictory();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (loaiCong == PortalType.RedPortal && other.CompareTag("Player1")) p1Win = false;
        else if (loaiCong == PortalType.BluePortal && other.CompareTag("Player2")) p2Win = false;
    }

    private void CheckVictory()
    {
        // ĐIỀU KIỆN THẮNG: Cả 2 đứng đúng cổng VÀ ĐÃ ĂN ĐỦ 2 CHÌA KHÓA
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