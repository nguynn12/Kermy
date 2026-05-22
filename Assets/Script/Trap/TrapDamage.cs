using UnityEngine;
using UnityEngine.SceneManagement; // Dùng để reset lại màn chơi khi chết

public class TrapDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật chạm vào bẫy có phải là Player không
        // Cách này hay ở chỗ: cả Ignus và Aqua đều nhận diện được nếu có chung Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + " đã chạm vào bẫy và hi sinh!");

            // Gọi hàm xử lý chết ở đây
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        // Tạm thời cho reset lại màn chơi hiện tại ngay lập tức khi chết
        // Sau này Mạnh có thể đổi thành chạy Animation chết rồi mới reset sau nhé!
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}