using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Vũ khí")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    [Header("Cài đặt Bắn")]
    public KeyCode shootKey;        
    public float fireRate = 0.3f;   
    
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(shootKey) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}