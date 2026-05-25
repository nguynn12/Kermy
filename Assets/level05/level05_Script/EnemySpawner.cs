    using System.Collections;
    using UnityEngine;

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject batPrefab;
        public GameObject waterDragonPrefab; // Chuẩn bị cho Wave 3
        public GameObject fireDragonPrefab;  // Chuẩn bị cho Wave 3
        public GameObject finalBossPrefab;

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


            // THÊM DÒNG NÀY ĐỂ SỬA LỖI: Đợi 1 giây khi vừa vào game
            yield return new WaitForSeconds(1f);

            // --- ĐỢT 3: TRIỆU HỒI HAI RỒNG MINI-BOSS ---
            Debug.Log("BẮT ĐẦU WAVE 3: Mini Boss Xuất Hiện!");
            SpawnMiniBosses();

            // 1. Chờ cho đến khi không còn con rồng nào trên màn hình (cả 2 đều đã chết)
            yield return new WaitUntil(() => FindObjectOfType<MiniBossHealth>() == null);

            // 2. Nghỉ 2 giây để màn hình trống trải, tạo sự căng thẳng
            yield return new WaitForSeconds(2f);

            // 3. Kích hoạt thước phim "Hợp Nhất" và gọi Wave 4
            yield return StartCoroutine(MergeDragonsAndSpawnBoss());
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
            // THAY ĐỔI Ở ĐÂY: Trừ đi 5f (thay vì 1f) để lùi Boss vào sâu bên trong màn hình
            float bossPosX = transform.position.x - 5f; 

            // Rồng nước ở trên, Rồng lửa ở dưới
            Vector3 waterBossPos = new Vector3(bossPosX, 2.5f, 0f);
            Vector3 fireBossPos = new Vector3(bossPosX, -2.5f, 0f);

            if (waterDragonPrefab != null) Instantiate(waterDragonPrefab, waterBossPos, Quaternion.identity);
            if (fireDragonPrefab != null) Instantiate(fireDragonPrefab, fireBossPos, Quaternion.identity);
        }
        // KỊCH BẢN ĐIỆN ẢNH: HỢP NHẤT RỒNG
    // KỊCH BẢN ĐIỆN ẢNH: HỢP NHẤT RỒNG & RUNG MÀN HÌNH
    IEnumerator MergeDragonsAndSpawnBoss()
    {
        Debug.Log("BẮT ĐẦU KỊCH BẢN HỢP NHẤT!");
        float bossPosX = transform.position.x - 5f; 
        
        Vector3 waterPos = new Vector3(bossPosX, 2.5f, 0f);
        Vector3 firePos = new Vector3(bossPosX, -2.5f, 0f);

        GameObject dummyWater = Instantiate(waterDragonPrefab, waterPos, Quaternion.identity);
        GameObject dummyFire = Instantiate(fireDragonPrefab, firePos, Quaternion.identity);

        Destroy(dummyWater.GetComponent<MiniBossAI>());
        Destroy(dummyWater.GetComponent<MiniBossHealth>());
        Destroy(dummyFire.GetComponent<MiniBossAI>());
        Destroy(dummyFire.GetComponent<MiniBossHealth>());

        yield return new WaitForSeconds(1f);

        // HIỆU ỨNG: RUNG MÀN HÌNH (Camera Shake)
        StartCoroutine(ShakeCamera(2f, 0.2f));

        Vector3 centerPos = new Vector3(bossPosX, 0f, 0f);
        float mergeSpeed = 3f; 

        while (dummyWater != null && Vector3.Distance(dummyWater.transform.position, centerPos) > 0.1f)
        {
            dummyWater.transform.position = Vector3.MoveTowards(dummyWater.transform.position, centerPos, mergeSpeed * Time.deltaTime);
            dummyFire.transform.position = Vector3.MoveTowards(dummyFire.transform.position, centerPos, mergeSpeed * Time.deltaTime);
            yield return null; 
        }

        Destroy(dummyWater);
        Destroy(dummyFire);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("BẮT ĐẦU WAVE 4: FINAL BOSS!");
        if (finalBossPrefab != null)
        {
            Instantiate(finalBossPrefab, centerPos, Quaternion.identity);
        }
    }

    // HÀM RUNG CAMERA
    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }
}