using UnityEngine;

// Singleton — tồn tại xuyên suốt các scene
// Gắn vào 1 GameObject tên "AudioManager" ở scene đầu tiên
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    // Static = truy cập từ bất kỳ script nào bằng AudioManager.Instance

    [Header("Nhạc nền")]
    [SerializeField] private AudioSource bgmSource;
    // AudioSource riêng cho nhạc nền — loop liên tục

    [Header("Âm thanh hiệu ứng")]
    [SerializeField] private AudioSource sfxSource;
    // AudioSource riêng cho SFX — phát 1 lần

    [Header("Danh sách âm thanh")]
    [SerializeField] private AudioClip bgmLevel1;
    [SerializeField] private AudioClip sfxTeleport;
    [SerializeField] private AudioClip sfxDoorOpen;
    [SerializeField] private AudioClip sfxKeyPickup;
    [SerializeField] private AudioClip sfxPlayerDie;
    [SerializeField] private AudioClip sfxLadderClimb;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Không bị xóa khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
            // Nếu đã có rồi → xóa cái mới
            return;
        }
    }

    private void Start()
    {
        PlayBGM(bgmLevel1);
    }

    // ── Phát nhạc nền ─────────────────────────────────────────
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip) return;
        // Không restart nếu đang phát bài đó rồi

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // ── Phát SFX ──────────────────────────────────────────────
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ── Các hàm gọi nhanh ─────────────────────────────────────
    public void PlayTeleport() => PlaySFX(sfxTeleport);
    public void PlayDoorOpen() => PlaySFX(sfxDoorOpen);
    public void PlayKeyPickup() => PlaySFX(sfxKeyPickup);
    public void PlayPlayerDie() => PlaySFX(sfxPlayerDie);
    public void PlayLadder() => PlaySFX(sfxLadderClimb);
    // => là cách viết ngắn thay vì { PlaySFX(sfx...); }
}