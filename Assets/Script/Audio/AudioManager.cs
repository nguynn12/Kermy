using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyVolumes();
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;

        if (clip != null)
        {
            bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume01)
    {
        bgmVolume = Mathf.Clamp01(volume01);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume01)
    {
        sfxVolume = Mathf.Clamp01(volume01);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}
