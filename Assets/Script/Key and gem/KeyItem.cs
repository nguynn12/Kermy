using UnityEngine;

public class KeyItem : MonoBehaviour
{
    // Chọn hệ cho chiếc chìa khóa này trên Inspector
    public PlayerInventory.ElementType keyElement;

    // ====================================================================
    // 🎵 Ô ĐỂ KÉO FILE ÂM THANH NHẶT KEY NGOÀI UNITY
    // ====================================================================
    [Header("Âm thanh Nhặt Key")]
    [SerializeField] private AudioClip keyPickupSound; 

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

                // 🎵 PHÁT TIẾNG PICKUP KEY NGAY TẠI ĐÂY (TRƯỚC KHI DESTROY)
                if (keyPickupSound != null)
                {
                    // Tạo loa ảo phát tiếng bíp bíp/ting ting của chìa khóa, âm lượng 0.7f cho rõ
                    AudioSource.PlayClipAtPoint(keyPickupSound, transform.position, 0.7f);
                }
                else
                {
                    Debug.LogWarning("Mạnh ơi, bạn chưa kéo file âm thanh vào ô Key Pickup Sound rồi!");
                }

                // ====================================================================
                // 🔑 NỐI VỚI CỔNG EXIT: Báo cho các cổng biết chìa khóa hệ này đã được nhặt
                // ====================================================================
                ExitPortal[] tatCaCong = FindObjectsByType<ExitPortal>(FindObjectsSortMode.None);
                foreach (ExitPortal cong in tatCaCong)
                {
                    // Truyền cả thông tin hệ của chìa khóa sang cho cổng xử lý luôn
                    cong.CollectKey(keyElement); 
                }
                
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