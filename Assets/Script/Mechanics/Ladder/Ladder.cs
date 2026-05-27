using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var controller = collision.GetComponentInParent<PlayerClimbController>();
        if (controller != null)
        {
            controller.SetCanClimb(true);
        }

        var climber = collision.GetComponentInParent<LadderClimber>();
        if (climber != null)
        {
            climber.EnterLadder(climbSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var controller = collision.GetComponentInParent<PlayerClimbController>();
        if (controller != null)
        {
            controller.SetCanClimb(false);
        }

        var climber = collision.GetComponentInParent<LadderClimber>();
        if (climber != null)
        {
            climber.ExitLadder();
        }
    }
}
