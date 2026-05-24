using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI Canvas Root")]
    [SerializeField] private GameObject pauseMenuRoot;

    // MỚI THÊM: Ô để kéo Nút Pause nhỏ vào
    [Header("Nút Pause Gameplay")]
    [SerializeField] private GameObject gameplayPauseButton;

    [Header("Cấu hình nút Âm thanh")]
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    [Header("Hiệu ứng Animator")]
    [SerializeField] private Animator pauseAnimator;

    public bool IsPaused { get; private set; }
    private static bool isMuted = false;

    private void Start()
    {
        ApplyState(false);
        SyncSoundState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame() => ApplyState(true);
    public void ResumeGame() => ApplyState(false);

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
        UpdateSoundIcon();
    }

    private void SyncSoundState()
    {
        AudioListener.pause = isMuted;
        UpdateSoundIcon();
    }

    private void UpdateSoundIcon()
    {
        if (soundButtonImage != null && soundOnSprite != null && soundOffSprite != null)
        {
            soundButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // Nơi xử lý ẩn/hiện mọi thứ
    private void ApplyState(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        // 1. Bật/Tắt Menu Pause
        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(paused);

            if (paused && pauseAnimator != null)
            {
                pauseAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                pauseAnimator.Play("PausePopOut", -1, 0f);
            }
        }

        // 2. Bật/Tắt Nút Pause Gameplay (Ngược lại với Menu)
        if (gameplayPauseButton != null)
        {
            // Nếu game đang paused (mở Menu) -> Ẩn nút đi (!true = false)
            // Nếu game đang chạy (tắt Menu) -> Hiện nút lên (!false = true)
            gameplayPauseButton.SetActive(!paused);
        }
    }
}