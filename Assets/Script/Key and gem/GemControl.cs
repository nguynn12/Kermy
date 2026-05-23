using UnityEngine;
using TMPro;

public class GemControl : MonoBehaviour
{
    public static GemControl Instance { get; private set; }

    [Header("=== GIAO DIỆN GEM CHÍNH GIỮA CANVAS ===")]
    public TextMeshProUGUI redGemText;
    public TextMeshProUGUI greenGemText;
    public TextMeshProUGUI blueGemText;

    private int _redCollected = 0;
    private int _greenCollected = 0;
    private int _blueCollected = 0;

    private int _redTotalInMap = 0;
    private int _greenTotalInMap = 0;
    private int _blueTotalInMap = 0;

    private int _totalGemsOnMap => (_redTotalInMap + _greenTotalInMap + _blueTotalInMap);
    private int _totalCollectedGems => (_redCollected + _greenCollected + _blueCollected);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (Instance != this) return;

        _redTotalInMap = 0;
        _greenTotalInMap = 0;
        _blueTotalInMap = 0;

        GemItem[] allGems = FindObjectsByType<GemItem>(FindObjectsSortMode.None);
        foreach (GemItem gem in allGems)
        {
            if (gem.gemColor == GemItem.GemColorType.Red) _redTotalInMap++;
            else if (gem.gemColor == GemItem.GemColorType.Green) _greenTotalInMap++;
            else if (gem.gemColor == GemItem.GemColorType.Blue) _blueTotalInMap++;
        }

        UpdateGemUI();
    }

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

    private void UpdateGemUI()
    {
        if (redGemText != null) 
            redGemText.text = $" {_redCollected}/{_redTotalInMap}";
        if (greenGemText != null) 
            greenGemText.text = $" {_greenCollected}/{_greenTotalInMap}";
        if (blueGemText != null) 
            blueGemText.text = $" {_blueCollected}/{_blueTotalInMap}";
    }

    public int CalculateStarsResult()
    {
        if (_totalGemsOnMap == 0) return 3;
        if (_totalCollectedGems == _totalGemsOnMap) return 3;
        else if (_totalCollectedGems >= _totalGemsOnMap / 2f) return 2;
        else return 1;
    }

    // ====================================================================
    // 🛠️ CHÈN THÊM: 2 hàm lấy điểm công khai để phục vụ bảng kết quả 5 sao
    // ====================================================================
    public int GetTotalCollectedGems()
    {
        return _totalCollectedGems;
    }

    public int GetTotalGemsOnMap()
    {
        return _totalGemsOnMap;
    }
}