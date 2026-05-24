using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tìm script leo trèo trên nhân vật (có thể ở object cha)
        var controller = collision.GetComponentInParent<PlayerClimbController>();
        if (controller != null)
            controller.SetCanClimb(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var controller = collision.GetComponentInParent<PlayerClimbController>();
        if (controller != null)
            controller.SetCanClimb(false);
    }
}