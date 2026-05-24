using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject batPrefab;
    public GameObject waterDragonPrefab; // Chuẩn bị cho Wave 3
    public GameObject fireDragonPrefab;  // Chuẩn bị cho Wave 3

    [Header("Cài đặt Đợt 1 (Wave 1)")]
    public float wave1Duration = 10f;     // Kéo dài đúng 10 giây
    public float wave1SpawnRate = 0.5f;   // Nửa giây ra 1 đợt
    public int minBatsPerSpawn = 2;       // Số dơi ÍT NHẤT mỗi nửa giây
    public int maxBatsPerSpawn = 4;       // Số dơi NHIỀU NHẤT mỗi nửa giây

    [Header("Cài đặt Đợt 2 (Wave 2)")]
    public int wave2Iterations = 10;      // 10 phiên vòng cung
    public float timeBetweenArcs = 2.5f;  // Thời gian giữa các phiên

    [Header("Giới hạn không gian")]
    public float minY = -4.5f;
    public float maxY = 4.5f;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        // --- ĐỢT 1: CƠN BÃO DƠI NGẪU NHIÊN 10 GIÂY ---
        Debug.Log("BẮT ĐẦU WAVE 1: Sinh tồn 10 giây!");
        float timer = 0f;
        
        while (timer < wave1Duration)
        {
            // Lấy ngẫu nhiên số lượng dơi sẽ thả ra trong đợt này (Ví dụ: từ 2 đến 4 con)
            int batsToSpawn = Random.Range(minBatsPerSpawn, maxBatsPerSpawn + 1);
            
            // Dùng vòng lặp để thả liền lúc nhiều con dơi
            for (int i = 0; i < batsToSpawn; i++)
            {
                SpawnSingleBat();
            }
            
            yield return new WaitForSeconds(wave1SpawnRate);
            timer += wave1SpawnRate; 
        }

        // Nghỉ 3 giây cho người chơi dọn dẹp màn hình
        yield return new WaitForSeconds(3f); 

        // --- ĐỢT 2: 10 PHIÊN VÒNG CUNG BAO VÂY ---
        Debug.Log("BẮT ĐẦU WAVE 2: Đội hình vòng cung!");
        for (int i = 0; i < wave2Iterations; i++)
        {
            SpawnArcFormation();
            yield return new WaitForSeconds(timeBetweenArcs);
        }

        // Nghỉ 4 giây trước khi Boss xuất hiện
        yield return new WaitForSeconds(4f); 

        // --- ĐỢT 3: TRIỆU HỒI HAI RỒNG MINI-BOSS ---
        Debug.Log("BẮT ĐẦU WAVE 3: Mini Boss Xuất Hiện!");
        SpawnMiniBosses();
    }

    void SpawnSingleBat()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0f);
        Instantiate(batPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnArcFormation()
    {
        float startX = transform.position.x;
        float centerY = Random.Range(-2f, 2f); 

        Vector3[] arcPositions = new Vector3[5];
        arcPositions[0] = new Vector3(startX + 1.5f, centerY + 3f, 0f);
        arcPositions[1] = new Vector3(startX + 0.5f, centerY + 1.5f, 0f);
        arcPositions[2] = new Vector3(startX, centerY, 0f); 
        arcPositions[3] = new Vector3(startX + 0.5f, centerY - 1.5f, 0f);
        arcPositions[4] = new Vector3(startX + 1.5f, centerY - 3f, 0f);

        foreach (Vector3 pos in arcPositions)
        {
            if (pos.y <= maxY && pos.y >= minY)
            {
                Instantiate(batPrefab, pos, Quaternion.identity);
            }
        }
    }

    void SpawnMiniBosses()
    {
        Vector3 waterBossPos = new Vector3(transform.position.x - 1f, 2.5f, 0f);
        Vector3 fireBossPos = new Vector3(transform.position.x - 1f, -2.5f, 0f);

        if (waterDragonPrefab != null) Instantiate(waterDragonPrefab, waterBossPos, Quaternion.identity);
        if (fireDragonPrefab != null) Instantiate(fireDragonPrefab, fireBossPos, Quaternion.identity);
    }
}