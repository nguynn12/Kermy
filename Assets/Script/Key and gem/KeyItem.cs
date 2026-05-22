using UnityEngine;

public class KeyItem : MonoBehaviour
{
    // Chọn hệ cho chiếc chìa khóa này trên Inspector
    public PlayerInventory.ElementType keyElement;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra vật chạm vào có script túi đồ không
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        
        if (inventory != null)
        {
            // Cực kỳ quan trọng: Kiểm tra xem hệ của Ếch có TRÙNG với hệ của Chìa khóa không
            if (inventory.playerElement == keyElement)
            {
                // Nếu trùng hệ thì cho nhặt
                inventory.CollectKey();
                
                // Xóa chiếc chìa khóa dưới đất đi (biến mất)
                Destroy(gameObject);
            }
            else
            {
                // Khác hệ thì không cho nhặt, hiện thông báo thử xem
                Debug.Log("Sai hệ rồi! Chìa khóa này không thuộc về bạn.");
            }
        }
    }
}