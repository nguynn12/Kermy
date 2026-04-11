using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Keys")]
    public int CollectedKeys { get; private set; }

    public event Action<int> OnKeyCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void LoadNextLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        int nextBuildIndex = current.buildIndex + 1;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount <= 0)
        {
            return;
        }

        if (nextBuildIndex >= sceneCount)
        {
            nextBuildIndex = sceneCount - 1;
        }

        SceneManager.LoadScene(nextBuildIndex);
    }

    public void RegisterKeyCollected()
    {
        CollectedKeys++;
        OnKeyCollected?.Invoke(CollectedKeys);
    }
}
