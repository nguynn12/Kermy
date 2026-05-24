using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Level01";
    [SerializeField] private GameObject optionsPanel;

    public void Configure(string sceneName, GameObject optionsRoot)
    {
        gameplaySceneName = sceneName;
        optionsPanel = optionsRoot;
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
            return;
        }

        SceneManager.LoadScene(1);
    }

    public void ShowOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void HideOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
