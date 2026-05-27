using UnityEngine;

// Gắn vào GameObject thang
// Cần: BoxCollider2D Is Trigger (bao phủ toàn bộ thang)
public class Ladders : MonoBehaviour
{
    [Header("Tốc độ leo")]
    [SerializeField] private float climbSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var climber = other.GetComponent<LadderClimber>();
        if (climber == null) return;
        climber.EnterLadder(climbSpeed);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var climber = other.GetComponent<LadderClimber>();
        if (climber == null) return;
        climber.ExitLadder();
    }
}
