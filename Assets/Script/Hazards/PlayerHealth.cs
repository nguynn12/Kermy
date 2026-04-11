using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void Kill()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
    }
}
