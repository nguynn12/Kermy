using System.Collections;
using UnityEngine;

public class FrogCutscene : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;    // THÊM DÒNG NÀY: Cái loa để phát nhạc
    public AudioClip victoryMusic;     // THÊM DÒNG NÀY: File nhạc ăn mừng
    [Header("Frog GameObjects")]
    public GameObject aqua, ignus, kermy;

    [Header("Animators")]
    public Animator aquaAnim, ignusAnim, kermyAnim;

    [Header("Target Positions")]
    public Transform landingPoint; // Điểm đáp xuống
    public Transform caveEntrance; // Cửa hang
    public Transform meetPoint;    // Điểm gặp nhau ăn mừng

    public float moveSpeed = 2f;

    void Start()
    {
        // Bắt đầu chạy cutscene ngay khi play game
        StartCoroutine(PlayCutsceneRoutine());
    }

    IEnumerator PlayCutsceneRoutine()
    {
        // 1. Aqua & Ignus bay và đáp xuống đất
        aquaAnim.Play("Aqua_Fly");
        ignusAnim.Play("Ignus_Fly");
        
        // Giả lập thời gian bay từ trên không xuống
        yield return StartCoroutine(MoveToPosition(aqua, landingPoint.position + new Vector3(-1, 0, 0)));
        yield return StartCoroutine(MoveToPosition(ignus, landingPoint.position + new Vector3(1, 0, 0)));

        // 2. Đi từ từ vào trước cửa hang, Kermy đi ra
        aquaAnim.Play("Aqua_Run");
        ignusAnim.Play("Ignus_Run");
        kermyAnim.Play("Kermy_Run");

        // Di chuyển cả 3 cùng lúc (không dùng yield để chúng đi song song)
        StartCoroutine(MoveToPosition(aqua, caveEntrance.position + new Vector3(-2, 0, 0)));
        StartCoroutine(MoveToPosition(ignus, caveEntrance.position + new Vector3(-1, 0, 0)));
        StartCoroutine(MoveToPosition(kermy, caveEntrance.position + new Vector3(1, 0, 0)));
        
        yield return new WaitForSeconds(2f); // Đợi chúng đi đến nơi

        // 3. Cả 3 đứng yên nhìn nhau 2-3 giây
        aquaAnim.Play("Aqua_Idle");
        ignusAnim.Play("Ignus_Idle");
        kermyAnim.Play("Kermy_Idle");
        
        yield return new WaitForSeconds(2.5f); // Đứng yên 2.5s

        // 4. Chạy vào gần nhau
        aquaAnim.Play("Aqua_Run");
        ignusAnim.Play("Ignus_Run");
        kermyAnim.Play("Kermy_Run");

        StartCoroutine(MoveToPosition(aqua, meetPoint.position + new Vector3(-1, 0, 0)));
        StartCoroutine(MoveToPosition(ignus, meetPoint.position));
        StartCoroutine(MoveToPosition(kermy, meetPoint.position + new Vector3(1, 0, 0)));

        yield return new WaitForSeconds(1f); // Đợi chạy lại gần

        // 5. Làm động tác ăn mừng
        aquaAnim.Play("Aqua_Victory");
        ignusAnim.Play("Ignus_Victory");
        kermyAnim.Play("Kermy_Victory");

        
      
        // THÊM ĐOẠN NÀY LÀ XONG: Phát đoạn nhạc ăn mừng
        if (audioSource != null && victoryMusic != null)
        {
            audioSource.PlayOneShot(victoryMusic);
        }
    
    }

    // Hàm hỗ trợ di chuyển GameObject mượt mà
    IEnumerator MoveToPosition(GameObject obj, Vector3 targetPos)
    {
        while (Vector3.Distance(obj.transform.position, targetPos) > 0.05f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        obj.transform.position = targetPos;
    }
}