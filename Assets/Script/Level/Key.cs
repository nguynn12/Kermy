using UnityEngine;

// Gắn vào GameObject chìa khóa
// Cần: SpriteRenderer, CircleCollider2D (Is Trigger), AudioSource
public class Key : MonoBehaviour
{
    [Header("Cổng sẽ mở khi lấy key này")]
    [SerializeField] private ExitDoor targetDoor;
    // Kéo ExitDoor vào đây trong Inspector

    [Header("Loại nguyên tố được phép lấy key")]
    [SerializeField] private ElementalType requiredElement;
    // Key lửa → chỉ Ignus lấy được
    // Key nước → chỉ Aqua lấy được

    [Header("Âm thanh khi lấy key")]
    [SerializeField] private AudioClip pickupSound;

    [Header("Hiệu ứng lơ lửng")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.15f;
    // Key sẽ nhấp nhô lên xuống trông sinh động hơn

    // ── Biến nội bộ ───────────────────────────────────────────
    private Vector3 _startPos;
    private AudioSource _audio;

    private void Awake()
    {
        _startPos = transform.position;
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Hiệu ứng lơ lửng — nhấp nhô theo sin wave
        float newY = _startPos.y
                   + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        // Mathf.Sin trả về -1 đến 1 theo chu kỳ
        // Nhân với floatHeight → biên độ dao động
        // Nhân với floatSpeed → tốc độ dao động

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        var identity = other.GetComponent<ElementalIdentity>();
        if (identity == null) return;

        // Kiểm tra đúng nguyên tố
        if (identity.Type != requiredElement) return;

        // Phát tiếng nhặt key
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        // PlayClipAtPoint = phát âm thanh tại vị trí, không bị mất
        // dù GameObject bị xóa ngay sau đó

        // Mở cửa
        if (targetDoor != null)
            targetDoor.Unlock();

        // Xóa key khỏi scene
        Destroy(gameObject);
    }
}