using System.Collections;
using UnityEngine;

// Script này KHÔNG cần gắn tay — TeleportPortal tự thêm vào nhân vật
public class TeleportStatus : MonoBehaviour
{
    public bool IsImmune { get; private set; }
    // true = đang trong cooldown, không bị teleport

    public void StartCooldown(float duration)
    {
        StopAllCoroutines();
        // Dừng cooldown cũ nếu đang chạy (tránh chồng chéo)

        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        IsImmune = true;
        // Bật miễn dịch ngay

        yield return new WaitForSeconds(duration);
        // Chờ hết thời gian cooldown

        IsImmune = false;
        // Tắt miễn dịch — có thể teleport lại bình thường
    }
}