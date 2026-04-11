using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private int requiredKeys = 1;
    [SerializeField] private Collider2D doorCollider;

    public bool IsOpen { get; private set; }

    private void Reset()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (IsOpen)
        {
            return;
        }

        if (LevelManager.Instance == null)
        {
            return;
        }

        if (LevelManager.Instance.CollectedKeys >= requiredKeys)
        {
            Open();
        }
    }

    private void Open()
    {
        IsOpen = true;
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
}
