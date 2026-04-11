using UnityEngine;
using UnityEngine.Events;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip sfxClip;

    [Header("Events")]
    [SerializeField] private UnityEvent onTriggered;

    public void Play()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxClip);
        }

        onTriggered?.Invoke();
    }
}
