using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // BẮT BUỘC THÊM DÒNG NÀY ĐỂ DÙNG UI IMAGE
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    [Header("Input (Chọn phím trực tiếp)")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("UI & Scene")]
    [SerializeField] private GameObject pauseMenuRoot;

    [Header("Cấu hình nút Âm thanh")]
    [Tooltip("Kéo cái Image của nút bấm Âm thanh vào đây")]
    public Image audioButtonImage;
    public Sprite soundOnSprite;  // Ảnh loa BẬT
    public Sprite soundOffSprite; // Ảnh loa TẮT (hình bạn gửi)

    [Header("Main Menu Scene")]
#if UNITY_EDITOR
    public SceneAsset mainMenuSceneAsset;
#endif
    [SerializeField, HideInInspector]
    private string mainMenuSceneName;

    public bool IsPaused { get; private set; }
    private bool isMuted = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (mainMenuSceneAsset != null) mainMenuSceneName = mainMenuSceneAsset.name;
        else mainMenuSceneName = "";
    }
#endif

    private void Start()
    {
        ApplyState(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (!IsPaused) PauseGame();
            else ResumeGame();
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void PauseGame()
    {
        ApplyState(true);
    }

    public void ResumeGame()
    {
        ApplyState(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (!string.IsNullOrEmpty(mainMenuSceneName)) SceneManager.LoadScene(mainMenuSceneName);
        else Debug.LogError("Bạn chưa kéo file Scene Main Menu vào PauseManager!");
    }

    // ==========================================
    // NÚT CHỦ ĐỘNG BẬT/TẮT ÂM THANH & ĐỔI ẢNH
    // ==========================================
    public void ToggleAudio()
    {
        isMuted = !isMuted; // Đảo trạng thái
        AudioListener.volume = isMuted ? 0f : 1f; // Tắt/Bật tiếng

        // Tráo đổi ảnh Sprite ngay lập tức
        if (audioButtonImage != null)
        {
            audioButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }

    private void ApplyState(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(paused);
    }
}