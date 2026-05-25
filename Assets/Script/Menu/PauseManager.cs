using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Bắt buộc phải có cái này để kéo thả file Scene
#endif

public class PauseManager : MonoBehaviour
{
    [Header("Input (Chọn phím trực tiếp)")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("UI & Scene")]
    [SerializeField] private GameObject pauseMenuRoot;

    [Header("Main Menu Scene")]
    [Tooltip("Kéo trực tiếp file Scene MainMenu từ cửa sổ Project thả vào đây")]
#if UNITY_EDITOR
    // Biến này hiển thị ở Inspector để bạn kéo thả file
    public SceneAsset mainMenuSceneAsset;
#endif

    // Biến này bị ẩn đi, tự động lưu tên Scene để Load khi chơi
    [SerializeField, HideInInspector]
    private string mainMenuSceneName;

    public bool IsPaused { get; private set; }

#if UNITY_EDITOR
    // Hàm này tự động chạy mỗi khi bạn thay đổi gì đó trên Inspector
    private void OnValidate()
    {
        if (mainMenuSceneAsset != null)
        {
            // Tự động lấy tên file Scene vừa kéo vào và lưu ngầm
            mainMenuSceneName = mainMenuSceneAsset.name;
        }
        else
        {
            mainMenuSceneName = "";
        }
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
    }

    public void PauseGame()
    {
        ApplyState(true);
    }

    public void ResumeGame()
    {
        ApplyState(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        // Load scene dựa trên tên đã được tự động lưu ngầm
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Bạn chưa kéo file Scene Main Menu vào PauseManager!");
        }
    }

    private void ApplyState(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(paused);
        }
    }
}