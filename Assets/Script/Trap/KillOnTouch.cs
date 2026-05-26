using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là con Ếch không (thông qua component PlayerHealth)
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        
        if (health != null)
        {
            Debug.Log(other.name + " đã va chạm với bẫy và chết!");

            // ====================================================================
            // 🔊 GỌI KHO NHẠC TỔNG: Tự lấy Tag của bẫy này gửi qua cho Manager phát nhạc
            // ====================================================================
            AudioSource loaCuaEch = other.GetComponent<AudioSource>();
            if (loaCuaEch == null) 
            {
                loaCuaEch = other.gameObject.AddComponent<AudioSource>();
                loaCuaEch.playOnAwake = false;
                loaCuaEch.spatialBlend = 0f;
            }

            if (TrapSoundManager.Instance != null)
            {
                // Truyền Tag của chính vùng va chạm này và cái loa của ếch qua cho Manager xử lý
                TrapSoundManager.Instance.PlayTrapSound(gameObject.tag, loaCuaEch);
            }
            // ====================================================================

            // 🔊 KÍCH HOẠT TIẾNG CHẾT: Cho con ếch kêu lên một tiếng đau đớn trước khi dịch chuyển
            PlayerController pControl = other.GetComponent<PlayerController>();
            if (pControl != null)
            {
                pControl.PlayDeathSound();
            }

            // DỌN DẸP VẬT LÝ: Triệt tiêu ngay vận tốc để con ếch không bị khựng đơ lơ lửng trên bẫy
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Xóa vận tốc chạy/rơi
                rb.angularVelocity = 0f;          // Xóa vận tốc xoay
            }

            // Gọi hàm Kill gốc trong dự án của bạn để đưa ếch về Checkpoint
            health.Kill(); 
        }
    }
}