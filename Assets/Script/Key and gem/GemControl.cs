using UnityEngine;
using TMPro;

public class GemControl : MonoBehaviour
{
    public static GemControl Instance { get; private set; }

    [Header("=== GIAO DIỆN GEM CHÍNH GIỮA CANVAS ===")]
    public TextMeshProUGUI redGemText;   // Ô nhận Text màu Đỏ
    public TextMeshProUGUI greenGemText; // Ô nhận Text màu Xanh Lá
    public TextMeshProUGUI blueGemText;  // Ô nhận Text màu Xanh Dương

    // Biến lưu trữ số lượng ĐÃ NHẶT được
    private int _redCollected = 0;
    private int _greenCollected = 0;
    private int _blueCollected = 0;

    // Biến tự động đếm TỔNG SỐ NGỌC CÓ TRÊN MAP theo từng màu
    private int _redTotalInMap = 0;
    private int _greenTotalInMap = 0;
    private int _blueTotalInMap = 0;

    // Thuộc tính tính tổng số ngọc để phục vụ hàm tính sao cuối màn
    private int _totalGemsOnMap => (_redTotalInMap + _greenTotalInMap + _blueTotalInMap);
    private int _totalCollectedGems => (_redCollected + _greenCollected + _blueCollected);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 🌟 TỰ ĐỘNG QUÉT VÀ PHÂN LOẠI MÀU NGỌC TRÊN MAP KHI VÀO GAME
        GemItem[] allGems = FindObjectsByType<GemItem>(FindObjectsSortMode.None);
        foreach (GemItem gem in allGems)
        {
            if (gem.gemColor == GemItem.GemColorType.Red) _redTotalInMap++;
            else if (gem.gemColor == GemItem.GemColorType.Green) _greenTotalInMap++;
            else if (gem.gemColor == GemItem.GemColorType.Blue) _blueTotalInMap++;
        }

        UpdateGemUI();
    }

    // ==========================================================
    // 🌟 CÁC HÀM CỘNG ĐIỂM RIÊNG BIỆT (Được gọi từ GemItem)
    // ==========================================================
    
    public void AddRedGem()
    {
        _redCollected++;
        UpdateGemUI();
    }

    public void AddGreenGem()
    {
        _greenCollected++;
        UpdateGemUI();
    }

    public void AddBlueGem()
    {
        _blueCollected++;
        UpdateGemUI();
    }

    // ==========================================================
    // 📺 HÀM CẬP NHẬT ĐỊNH DẠNG CHỮ SIÊU TRỰC QUAN
    // ==========================================================
    private void UpdateGemUI()
    {
        // Hiển thị rõ ràng chữ Lửa kèm tỉ lệ ngọc đỏ (Ví dụ: Lửa: 1/3)
        if (redGemText != null) 
            redGemText.text = $" {_redCollected}/{_redTotalInMap}";

        // Hiển thị rõ ràng chữ Ngọc kèm tỉ lệ ngọc xanh lá (Ví dụ: Ngọc: 0/2)
        if (greenGemText != null) 
            greenGemText.text = $" {_greenCollected}/{_greenTotalInMap}";

        // Hiển thị rõ ràng chữ Nước kèm tỉ lệ ngọc xanh dương (Ví dụ: Nước: 2/4)
        if (blueGemText != null) 
            blueGemText.text = $" {_blueCollected}/{_blueTotalInMap}";
    }

    // ==========================================================
    // ⭐ HÀM TÍNH TOÁN SỐ SAO ĐẠT ĐƯỢC KHI QUA MÀN
    // ==========================================================
    public int CalculateStarsResult()
    {
        if (_totalGemsOnMap == 0) return 3; 
        if (_totalCollectedGems == _totalGemsOnMap) return 3; // Ăn sạch bách ngọc được 3 sao
        else if (_totalCollectedGems >= _totalGemsOnMap / 2f) return 2; // Ăn được một nửa trở lên được 2 sao
        else return 1; // Ăn ít quá được 1 sao
    }
}