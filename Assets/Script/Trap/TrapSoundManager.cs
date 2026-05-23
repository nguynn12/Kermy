using UnityEngine;

public class TrapSoundManager : MonoBehaviour
{
    // Tạo cơ chế Singleton để các script khác gọi nhanh trong 1 dòng lệnh
    public static TrapSoundManager Instance { get; private set; }

    [Header("=== KÉO TẤT CẢ FILE ÂM THANH BẪY VÀO ĐÂY ===")]
    [SerializeField] private AudioClip baySapSound;   // Tiếng bẫy đập/sập
    [SerializeField] private AudioClip bayXoaySound;  // Tiếng bẫy xoay/rìu
    [SerializeField] private AudioClip bayNhoSound;   // Tiếng bẫy gai nhô

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Hàm trung gian nhận Tag từ bẫy và mượn loa của Ếch để phát nhạc 2D
    public void PlayTrapSound(string trapTag, AudioSource playerAudio)
    {
        AudioClip clipToPlay = null;

        // Dựa vào Tag của vùng va chạm để chọn đúng bài nhạc
        if (trapTag == "TrapSap") clipToPlay = baySapSound;
        else if (trapTag == "TrapXoay") clipToPlay = bayXoaySound;
        else if (trapTag == "TrapNho") clipToPlay = bayNhoSound;

        // Phát nhạc bằng loa của Ếch để nghe to rõ, không lỗi khoảng cách
        if (clipToPlay != null && playerAudio != null)
        {
            playerAudio.PlayOneShot(clipToPlay, 0.8f);
        }
    }
}