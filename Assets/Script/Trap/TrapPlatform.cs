using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapPlatform : MonoBehaviour
{
    [Header("Settings")]
    public string playerLayerName = "Player";
    [Tooltip("Thời gian đứng im tối đa trên bẫy trước khi tự động sập")]
    public float delayBeforeFall = 3f;
    [Tooltip("Thời gian chờ cực ngắn sau khi bấm nhảy để lực nhảy kịp đẩy ếch lên rồi mới rụng bẫy")]
    public float delayAfterJump = 0.05f;
    public float resetDelay = 3f;

    private bool isFalling = false;
    private Vector3 startPos;
    private Coroutine fallCoroutine;

    // Quản lý danh sách các PlayerController đang đứng trên bẫy
    private List<PlayerController> playersOnPlatform = new List<PlayerController>();

    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isFalling) return;

        // Quét liên tục mỗi khung hình xem có con ếch nào bấm nút nhảy không
        foreach (PlayerController player in playersOnPlatform)
        {
            if (player != null)
            {
                // Bảo lãnh trạng thái Grounded để con ếch luôn ăn nút nhảy cực nhạy
                player.ForceGroundedFromTrap(true);

                // 🔥 ĐIỀU KIỆN 1: Nếu người chơi chủ động BẤM NHẢY -> Cho SẬP LẬP TỨC!
                if (player.IsJumpRequested)
                {
                    // Hủy ngay bộ đếm chờ 3 giây cũ
                    if (fallCoroutine != null) StopCoroutine(fallCoroutine);
                    
                    // Kích hoạt lệnh sập ngay lập tức
                    fallCoroutine = StartCoroutine(FallSequenceImmediate());
                    break;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(playerLayerName)) return;
        if (isFalling) return;

        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        if (player != null && !playersOnPlatform.Contains(player))
        {
            playersOnPlatform.Add(player);
        }

        // Khi có con ếch đầu tiên bước lên bẫy, bắt đầu đếm ngược 3 giây
        if (fallCoroutine == null)
        {
            fallCoroutine = StartCoroutine(FallSequenceStandard());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(playerLayerName)) return;
        if (isFalling) return;

        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        if (player != null && playersOnPlatform.Contains(player))
        {
            player.ForceGroundedFromTrap(false);
            playersOnPlatform.Remove(player);
        }

        // Nếu cả hai con ếch đều chạy ra khỏi bẫy trước khi nó kịp sập, hủy bộ đếm 3 giây để reset bẫy an toàn
        if (fallCoroutine != null && playersOnPlatform.Count == 0)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }
    }

    // 🔥 ĐIỀU KIỆN 2: Bộ đếm tiêu chuẩn (Đứng im lờ đờ quá 3 giây tự động rụng cái rầm)
    IEnumerator FallSequenceStandard()
    {
        yield return new WaitForSeconds(delayBeforeFall);
        TriggerFall();
    }

    // Bộ đếm sập siêu tốc khi phát hiện có cú nhảy
    IEnumerator FallSequenceImmediate()
    {
        yield return new WaitForSeconds(delayAfterJump);
        TriggerFall();
    }

    // Hàm xử lý kích hoạt vật lý sập khối gạch xuống vực
    void TriggerFall()
    {
        isFalling = true;

        // Tắt toàn bộ bảo lãnh nhảy trước khi bẫy biến mất để tránh lỗi lơ lửng
        foreach (PlayerController player in playersOnPlatform)
        {
            if (player != null) player.ForceGroundedFromTrap(false);
        }
        playersOnPlatform.Clear();

        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        // Bắt đầu đếm giây để hồi sinh lại khối gạch về chỗ cũ
        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(resetDelay);
        ResetPlatform();
    }

    void ResetPlatform()
    {
        isFalling = false;
        playersOnPlatform.Clear();
        transform.position = startPos;
        GetComponent<Collider2D>().enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        fallCoroutine = null;
    }
}