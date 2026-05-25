using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections.Generic;

public class DayNightTransformer : MonoBehaviour
{
    [Header("--- HỆ THỐNG BACKGROUND ---")]
    public SpriteRenderer skyDayBackground;
    public SpriteRenderer skyNightBackground;

    [Header("--- ĐÈN CHÍNH (BIẾN HÌNH) ---")]
    public Light2D mainLight;

    [Header("--- VẬT THỂ ẨN/HIỆN THEO NGÀY ĐÊM ---")]
    [Tooltip("Kéo GameObject BlockNight ở Hierarchy vào đây")]
    public GameObject blockNight; // Thêm biến này để chứa khối BlockNight

    [Header("--- THIẾT LẬP BAN NGÀY ---")]
    public Color dayColor = new Color(0.95f, 0.95f, 0.85f);
    public float dayIntensity = 1f;

    [Header("--- THIẾT LẬP BAN ĐÊM ---")]
    public Color nightColor = new Color(0.4f, 0.5f, 0.7f);
    public float nightIntensity = 1f;
    public float nightFalloff = 7f;
    [Range(0f, 1f)] public float nightFalloffStrength = 0.5f;

    [Header("--- CẤU HÌNH TƯƠNG TÁC UI ---")]
    [Tooltip("Kéo cái Button gợi ý trên màn hình vào đây")]
    public GameObject interactUI;
    [Tooltip("Kéo cái Text (TMP) nằm trong Button vào đây")]
    public TextMeshProUGUI interactText;

    [Header("--- PHÍM BẤM THEO NHÂN VẬT ---")]
    public string character1Name = "Ignus";
    public KeyCode character1Key = KeyCode.E;
    public string character2Name = "Aqua";
    public KeyCode character2Key = KeyCode.Alpha0;

    private bool isDay = true;
    private GameObject currentOccupant;
    private KeyCode currentActiveKey = KeyCode.None;
    private List<GameObject> playersInArea = new List<GameObject>();

    void Start()
    {
        SetDayMode();
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        // Chỉ cho tương tác nếu có người chiếm quyền và ấn đúng phím
        if (currentOccupant != null && Input.GetKeyDown(currentActiveKey))
        {
            InteractWithLever();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. XỬ LÝ ĐỔI NGÀY ĐÊM VÀ HIỂN THỊ NÚT UI
            if (!playersInArea.Contains(collision.gameObject))
                playersInArea.Add(collision.gameObject);

            if (currentOccupant == null)
                BindCharacter(collision.gameObject);

            // 2. TÍCH HỢP TÌM SCRIPT LEO TRÈO TRÊN NHÂN VẬT (BẬT LEO)
            var controller = collision.GetComponentInParent<PlayerClimbController>();
            if (controller != null)
            {
                controller.SetCanClimb(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. XỬ LÝ ĐỔI NGÀY ĐÊM VÀ ẨN NÚT UI
            playersInArea.Remove(collision.gameObject);

            if (collision.gameObject == currentOccupant)
            {
                if (playersInArea.Count > 0)
                    BindCharacter(playersInArea[0]);
                else
                    ClearInteraction();
            }
            else if (playersInArea.Count == 0)
            {
                ClearInteraction();
            }

            // 2. TÍCH HỢP TÌM SCRIPT LEO TRÈO TRÊN NHÂN VẬT (TẮT LEO)
            var controller = collision.GetComponentInParent<PlayerClimbController>();
            if (controller != null)
            {
                controller.SetCanClimb(false);
            }
        }
    }

    private void BindCharacter(GameObject targetCharacter)
    {
        currentOccupant = targetCharacter;

        // Xác định phím dựa trên tên nhân vật
        if (targetCharacter.name == character1Name)
            currentActiveKey = character1Key;
        else if (targetCharacter.name == character2Name)
            currentActiveKey = character2Key;
        else
            currentActiveKey = KeyCode.E; // fallback

        // Cập nhật trạng thái hiển thị UI (Vị trí giữ nguyên trên màn hình)
        if (interactUI != null)
        {
            interactUI.SetActive(true);
        }

        if (interactText != null)
            UpdateUIText(currentActiveKey);
    }

    private void ClearInteraction()
    {
        currentOccupant = null;
        currentActiveKey = KeyCode.None;
        if (interactUI != null) interactUI.SetActive(false);
    }

    private void UpdateUIText(KeyCode key)
    {
        if (interactText == null) return;
        string keyName = key.ToString();
        if (keyName.StartsWith("Alpha")) keyName = keyName.Replace("Alpha", "");
        interactText.text = keyName;
    }

    public void InteractWithLever()
    {
        isDay = !isDay;
        if (isDay) SetDayMode();
        else SetNightMode();
    }

    private void SetDayMode()
    {
        skyDayBackground.sortingOrder = -10;
        skyNightBackground.sortingOrder = -11;
        if (mainLight != null)
        {
            mainLight.lightType = Light2D.LightType.Global;
            mainLight.color = dayColor;
            mainLight.intensity = dayIntensity;
        }

        // TẮT KHỐI BLOCK KHI TRỜI SÁNG
        if (blockNight != null)
        {
            blockNight.SetActive(false);
        }
    }

    private void SetNightMode()
    {
        skyDayBackground.sortingOrder = -11;
        skyNightBackground.sortingOrder = -10;
        if (mainLight != null)
        {
            mainLight.lightType = Light2D.LightType.Freeform;
            mainLight.color = nightColor;
            mainLight.intensity = nightIntensity;
            mainLight.shapeLightFalloffSize = nightFalloff;
            mainLight.falloffIntensity = nightFalloffStrength;
        }

        // BẬT KHỐI BLOCK KHI TRỜI TỐI
        if (blockNight != null)
        {
            blockNight.SetActive(true);
        }
    }
}