    using System.Collections;
    using UnityEngine;

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Cinematic Sound")]
        public AudioClip earthquakeSound; // Chứa file tiếng động đất/rung màn hình 
        public AudioClip bossRoarSound;   // Tiếng Boss gầm thét
        
        [Header("Nhạc Nền (BGM)")]
        public AudioSource bgmSource; // Chứa cái BGM_Manager ngoài màn hình
        public AudioClip bossBGM;     // File nhạc kịch tính lúc đánh Boss

        [Header("Prefabs")]
        public GameObject batPrefab;
        public GameObject waterDragonPrefab; // Chuẩn bị cho Wave 3
        public GameObject fireDragonPrefab;  // Chuẩn bị cho Wave 3
        public GameObject finalBossPrefab;
        public GameObject shieldItemPrefab;

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
        // 1. SỬA LỖI: Đợi 1 giây khi vừa vào game (Đã chuyển lên đúng vị trí đầu tiên)
        yield return new WaitForSeconds(1f);

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

        // 1. Chờ cho đến khi không còn con rồng nào trên màn hình (cả 2 đều đã chết)
        yield return new WaitUntil(() => FindObjectOfType<MiniBossHealth>() == null);

        // 2. Nghỉ 2 giây để màn hình trống trải, tạo sự căng thẳng
        yield return new WaitForSeconds(2f);

        // --- ĐỢT 4: KÍCH HOẠT HỢP NHẤT VÀ ĐÁNH BOSS CUỐI ---
        Debug.Log("BẮT ĐẦU WAVE 4: Trận chiến cuối cùng!");
        yield return StartCoroutine(MergeDragonsAndSpawnBoss());
        
        // (Đã xóa dòng test nhanh đợt 4 ở đây để tránh lỗi lặp vòng)
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
       // KỊCH BẢN: RUNG MÀN HÌNH -> HỢP NHẤT -> RA BOSS
IEnumerator MergeDragonsAndSpawnBoss()
    {
        // --- 0. TẮT NHẠC CŨ TẠO SỰ IM LẶNG ĐÁNG SỢ ---
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
        // --- 1. GIAI ĐOẠN ĐỘNG ĐẤT ---
        if (earthquakeSound != null)
        {
            AudioSource.PlayClipAtPoint(earthquakeSound, Camera.main.transform.position, 1f);
        }
        
        StartCoroutine(ShakeCamera(1.5f, 0.3f));
        yield return new WaitForSeconds(1.5f);

        float bossPosX = transform.position.x - 5f;
        Vector3 centerPos = new Vector3(bossPosX, 0f, 0f);

        // --- 2. GIAI ĐOẠN TRIỆU HỒI RỒNG DUMMY ---
        GameObject dWater = Instantiate(waterDragonPrefab, new Vector3(bossPosX, 3f, 0f), Quaternion.identity);
        GameObject dFire = Instantiate(fireDragonPrefab, new Vector3(bossPosX, -3f, 0f), Quaternion.identity);
        
        Destroy(dWater.GetComponent<MiniBossHealth>()); 
        Destroy(dFire.GetComponent<MiniBossHealth>());

        // --- 3. GIAI ĐOẠN HÚT VÀO NHAU TẠO CHỚP SÁNG ---
        while (dWater != null && Vector3.Distance(dWater.transform.position, centerPos) > 0.1f)
        {
            dWater.transform.position = Vector3.MoveTowards(dWater.transform.position, centerPos, 4f * Time.deltaTime);
            dFire.transform.position = Vector3.MoveTowards(dFire.transform.position, centerPos, 4f * Time.deltaTime);
            yield return null;
        }

        Destroy(dWater); 
        Destroy(dFire);
     
        // --- 4. GIAI ĐOẠN BOSS XUẤT HIỆN & ĐỨNG GẦM 5 GIÂY ---
        GameObject activeBoss = null;
        FinalBossAI bossAI = null;

        if (finalBossPrefab != null)
        {
            // Sinh ra Boss thật và lưu nó vào một biến
            activeBoss = Instantiate(finalBossPrefab, centerPos, Quaternion.identity);
            

            Collider2D bossCol = activeBoss.GetComponent<Collider2D>();
            if (bossCol != null) bossCol.enabled = false;

            // Tìm cái bộ não AI của con Boss vừa đẻ ra và TẠM TẮT NÓ ĐI
            bossAI = activeBoss.GetComponent<FinalBossAI>();
            if (bossAI != null)
            {
                bossAI.enabled = false; 
            }
        }

        yield return new WaitForSeconds(0.2f); // Nghỉ 0.2s cho ánh sáng tan bớt

       // ĐỔI NHẠC: Bật nhạc nền Boss kịch tính lên!
        if (bgmSource != null && bossBGM != null)
        {
            bgmSource.clip = bossBGM; // Tráo file nhạc
            bgmSource.volume = 1f;
            bgmSource.Play();         // Phát nhạc mới
        }
       

        // Phát tiếng gầm với âm lượng lớn nhất
        if (bossRoarSound != null)
        {
            AudioSource.PlayClipAtPoint(bossRoarSound, Camera.main.transform.position, 1f);
            Debug.Log("BOSS ĐANG GẦM THÉT TRONG 2 GIÂY!!!");
        }

        // CHỜ ĐÚNG 2 GIÂY (Để Boss đứng im biểu diễn)
        yield return new WaitForSeconds(2f);

        // HẾT 2 GIÂY: Bật bộ não AI lên lại để nó bắt đầu lùi về vị trí và xả đạn
        if (bossAI != null)
        {
            bossAI.enabled = true;
            Debug.Log("BOSS BẮT ĐẦU CHIẾN ĐẤU!");
        }

        // --- 5. GIAI ĐOẠN THẢ KHIÊN CỨU TRỢ ---
        if (shieldItemPrefab != null)
        {
            Instantiate(shieldItemPrefab, Vector3.zero, Quaternion.identity); 
            Debug.Log("KHIÊN CỨU TRỢ ĐÃ XUẤT HIỆN!");
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