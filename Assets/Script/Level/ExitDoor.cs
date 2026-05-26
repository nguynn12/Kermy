using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Gắn vào GameObject cổng thoát
// Cần: SpriteRenderer, BoxCollider2D (Is Trigger), AudioSource
public class ExitDoor : MonoBehaviour
{
    [Header("Sprite trạng thái")]
    [SerializeField] private Sprite spriteClosed; // kéo frame ĐÓNG vào
    [SerializeField] private Sprite spriteOpen;   // kéo frame MỞ vào

    [Header("Scene tiếp theo")]
    [SerializeField] private string nextSceneName;
    // Điền tên scene tiếp theo, VD: "Level02"

    [Header("Loại nguyên tố — Fire hoặc Water")]
    [SerializeField] private ElementalType requiredElement;
    // Fire Door → đặt Fire
    // Water Door → đặt Water

    [Header("Âm thanh")]
    [SerializeField] private AudioClip soundOpen;   // tiếng cửa mở
    [SerializeField] private AudioClip soundEnter;  // tiếng bước vào

    // ── Biến nội bộ ───────────────────────────────────────────
    private bool _isUnlocked = false;
    // true = có Key rồi, có thể mở

    private bool _isOpen = false;
    // true = cửa đang mở (nhân vật đúng nguyên tố đang đứng trong)

    private SpriteRenderer _sprite;
    private AudioSource _audio;
    private int _playerInsideCount = 0;
    // Đếm số nhân vật đang đứng trong trigger

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _audio = GetComponent<AudioSource>();

        // Bắt đầu ở trạng thái đóng
        SetDoorVisual(false);
    }

    // ── Nhận Key từ bên ngoài ─────────────────────────────────
    public void Unlock()
    {
        if (_isUnlocked) return;
        _isUnlocked = true;

        // Phát tiếng mở cửa
        PlaySound(soundOpen);

        // Nếu đang có người đứng trong → mở luôn
        if (_playerInsideCount > 0)
            TryOpen();
    }

    // ── Khi nhân vật bước vào trigger ─────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidPlayer(other)) return;

        _playerInsideCount++;

        if (_isUnlocked)
            TryOpen();
    }

    // ── Khi nhân vật rời trigger ──────────────────────────────
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidPlayer(other)) return;

        _playerInsideCount--;
        if (_playerInsideCount < 0) _playerInsideCount = 0;

        // Nếu không còn ai → đóng lại
        if (_playerInsideCount == 0)
        {
            _isOpen = false;
            SetDoorVisual(false);
        }
    }

    // ── Thử mở cửa ────────────────────────────────────────────
    private void TryOpen()
    {
        if (_isOpen) return;
        _isOpen = true;
        SetDoorVisual(true);
        PlaySound(soundEnter);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        // Chờ 1 giây cho animation/âm thanh xong

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // ── Đổi sprite đóng/mở ────────────────────────────────────
    private void SetDoorVisual(bool open)
    {
        if (_sprite == null) return;
        _sprite.sprite = open ? spriteOpen : spriteClosed;
    }

    // ── Kiểm tra nhân vật có đúng nguyên tố không ─────────────
    private bool IsValidPlayer(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return false;
        // Không phải nhân vật → bỏ qua

        var identity = other.GetComponent<ElementalIdentity>();
        if (identity == null) return false;

        return identity.Type == requiredElement;
        // Fire Door → chỉ Ignus vào được
        // Water Door → chỉ Aqua vào được
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audio != null && clip != null)
            _audio.PlayOneShot(clip);
        // PlayOneShot = phát 1 lần, không ngắt âm thanh đang chạy
    }
}