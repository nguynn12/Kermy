using UnityEngine;

// Gắn vào GameObject thang
// Yêu cầu: có BoxCollider2D Is Trigger
public class Ladder : MonoBehaviour
{
    [Header("Tốc độ leo thang")]
    [SerializeField] private float climbSpeed = 4f;

    // Khi nhân vật bước vào vùng thang
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;
        // Không phải nhân vật → bỏ qua

        var climber = other.GetComponent<LadderClimber>();
        if (climber == null) return;

        climber.EnterLadder(climbSpeed);
        // Báo cho nhân vật biết đang đứng trong thang
    }

    // Khi nhân vật rời khỏi vùng thang
    private void OnTriggerExit2D(Collider2D other)
    {
        var climber = other.GetComponent<LadderClimber>();
        if (climber == null) return;

        climber.ExitLadder();
        // Báo cho nhân vật biết đã ra khỏi thang
    }
}