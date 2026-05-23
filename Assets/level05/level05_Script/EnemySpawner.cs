using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Danh sách Quái và Chướng ngại vật")]
    public GameObject[] enemyPrefabs; 

    [Header("Cài đặt Nhịp độ")]
    public float spawnDelay = 2f; 
    
    [Header("Giới hạn chiều dọc (Trục Y)")]
    public float minY = -4f;      
    public float maxY = 4f;       

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnDelay; 
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return; 

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

        Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}