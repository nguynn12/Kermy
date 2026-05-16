using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Vector3 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.position;
    }

    public void Kill()
    {
        transform.position = spawnPosition;
    }
}