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

        if (!string.IsNullOrWhiteSpace(gameplaySceneName) && IsSceneInBuild(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
            return;
        }

        string firstLevelScene = FindFirstLevelSceneInBuild();
        if (!string.IsNullOrEmpty(firstLevelScene))
        {
            SceneManager.LoadScene(firstLevelScene);
        }
        else
        {
            Debug.LogError("No Level scene is available in Build Profiles.");
        }
    }

    private static bool IsSceneInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (buildSceneName == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    private static string FindFirstLevelSceneInBuild()
    {
        string bestScene = null;
        int bestLevel = int.MaxValue;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!TryParseLevelNumber(sceneName, out int levelNumber))
            {
                continue;
            }

            if (levelNumber < bestLevel)
            {
                bestLevel = levelNumber;
                bestScene = sceneName;
            }
        }

        return bestScene;
    }

    private static bool TryParseLevelNumber(string sceneName, out int levelNumber)
    {
        levelNumber = 0;

        if (string.IsNullOrEmpty(sceneName) || !sceneName.StartsWith("Level", System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string digits = sceneName.Substring("Level".Length);
        return int.TryParse(digits, out levelNumber);
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
