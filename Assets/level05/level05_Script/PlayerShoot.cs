using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private enum ControlProfile
    {
        Auto,
        Player1,
        Player2,
        Custom
    }

    [Header("Audio")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Weapon")]
    [SerializeField] private ControlProfile controlProfile = ControlProfile.Auto;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Shoot")]
    public KeyCode shootKey;
    public float fireRate = 0.3f;

    private float nextFireTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(ResolveShootKey()) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private KeyCode ResolveShootKey()
    {
        ControlProfile profile = ResolveProfile();
        if (profile == ControlProfile.Player1) return KeyCode.Q;
        if (profile == ControlProfile.Player2) return KeyCode.Space;
        return shootKey;
    }

    private ControlProfile ResolveProfile()
    {
        if (controlProfile != ControlProfile.Auto)
        {
            return controlProfile;
        }

        string normalizedName = gameObject.name.Replace(" ", string.Empty).ToLowerInvariant();
        if (normalizedName.Contains("player2") || normalizedName.Contains("aqua"))
        {
            return ControlProfile.Player2;
        }

        return ControlProfile.Player1;
    }
}
