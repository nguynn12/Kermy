using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject aboutCanvas;

    [Header("Scene Selection (kéo thả scene vào)")]
    // Chỉ hiển thị trong Editor, giúp chọn scene dễ dàng
#if UNITY_EDITOR
    public SceneAsset targetScene;
#endif
    [Tooltip("Tên scene sẽ load (tự động lấy từ targetScene)")]
    public string sceneName = "Level01";

    private void Start()
    {
        if (aboutCanvas != null)
            aboutCanvas.SetActive(false);

        // Nếu đã có SceneAsset trong Editor, tự động cập nhật sceneName
#if UNITY_EDITOR
        if (targetScene != null)
            sceneName = targetScene.name;
#endif
    }

    // Hàm PlayGame – có kiểm tra scene tồn tại
    public void PlayGame()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Chưa gán scene cần load! Hãy kéo scene vào ô targetScene hoặc gõ tên scene trong sceneName.");
            return;
        }

        // Kiểm tra scene có trong Build Settings không
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' chưa được thêm vào Build Settings. Vào File → Build Settings → Add Open Scenes.");
            return;
        }

        Debug.Log($"Đang load scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Kiểm tra scene có tồn tại trong Build Settings
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
                return true;
        }
        return false;
    }

    public void OpenAbout()
    {
        if (aboutCanvas != null) aboutCanvas.SetActive(true);
    }

    public void CloseAbout()
    {
        if (aboutCanvas != null) aboutCanvas.SetActive(false);
    }

    public void OpenOptions()
    {
        Debug.Log("Option Menu chưa làm tới, để tạm nhé!");
    }

    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}