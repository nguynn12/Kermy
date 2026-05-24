using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Input (Chọn phím trực tiếp)")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("UI & Scene")]
    [SerializeField] private GameObject pauseMenuRoot;
    [Tooltip("Nhập tên Scene hoặc Index của màn hình Main Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

    private void Start()
    {
        // Đảm bảo menu ẩn khi bắt đầu
        ApplyState(false);
    }

    private void Update()
    {
        // Kiểm tra phím tắt thủ công
        if (Input.GetKeyDown(pauseKey))
        {
            if (!IsPaused) PauseGame();
            else ResumeGame();
        }
    }

    private void OnDisable()
    {
        // Đảm bảo game chạy lại khi thoát scene hoặc script bị tắt
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

        // Kiểm tra xem mainMenuSceneName là số hay tên để load
        if (int.TryParse(mainMenuSceneName, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
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