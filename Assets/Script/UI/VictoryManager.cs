using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance { get; private set; }

    [Header("=== GIAO DIỆN BẢNG CHIẾN THẮNG ===")]
    [SerializeField] private GameObject victoryPanel; 
    [SerializeField] private TextMeshProUGUI gemResultText; 
    
    [Header("=== HỆ THỐNG UI 5 NGÔI SAO ===")]
    [SerializeField] private Image[] starImages; // Kéo 5 Object Ngôi Sao ngoài UI vào đây
    [SerializeField] private Sprite fullStarSprite;  // Ảnh ngôi sao màu vàng đặc
    [SerializeField] private Sprite emptyStarSprite; // Ảnh ngôi sao màu xám rỗng

    [Header("Màn chơi tiếp theo")]
    [SerializeField] private string nextSceneName = "MenuGame";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void ShowVictoryScreen(int collected, int total)
    {
        if (victoryPanel == null) return;

        victoryPanel.SetActive(true);

        if (gemResultText != null)
        {
            gemResultText.text = $"Ngọc đã nhặt: {collected} / {total}";
        }

        // 🌟 TÍNH TOÁN SỐ SAO THEO CÔNG THỨC MẠNH YÊU CẦU
        int starsEarned = 0;

        if (total == 0) 
        {
            starsEarned = 5; 
        }
        else if (collected == 0)
        {
            starsEarned = 0; // Không ăn được viên nào: Bị trừ đi sạch bách điểm sao!
        }
        else if (collected == total)
        {
            starsEarned = 5; // Ăn sạch bách ngọc trên map: Được 5 sao cực đỉnh!
        }
        else
        {
            // Chia đều tỷ lệ phần trăm từ 1 đến 4 sao
            float ratio = (float)collected / total;
            if (ratio >= 0.75f) starsEarned = 4;
            else if (ratio >= 0.5f) starsEarned = 3;
            else if (ratio >= 0.25f) starsEarned = 2;
            else starsEarned = 1;
        }

        // Cập nhật đổi hình ảnh sao trên Canvas ngoài Unity
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].sprite = (i < starsEarned) ? fullStarSprite : emptyStarSprite;
            }
        }
    }

    // Hàm gắn vào Nút Bấm "Tiếp Tục" ngoài UI để chuyển sang màn tiếp theo
    public void ButtonNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}