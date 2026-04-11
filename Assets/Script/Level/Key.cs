using UnityEngine;

public class Key : MonoBehaviour
{
    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterKeyCollected();
        }

        _collected = true;
        gameObject.SetActive(false);
    }
}
