using UnityEngine;

public class TeleportCameraHandler : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera rightCamera;

    private void Awake()
    {
        RestoreSingleCamera();
    }

    public void OnPlayerTeleported()
    {
        RestoreSingleCamera();

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }

        if (cameraController != null)
        {
            cameraController.enabled = true;
            cameraController.FramePlayersImmediately();
        }
    }

    private void RestoreSingleCamera()
    {
        if (cameraController == null)
        {
            cameraController = GetComponent<CameraController>();
        }

        if (rightCamera == null)
        {
            rightCamera = GetComponent<Camera>();
        }

        if (rightCamera != null)
        {
            rightCamera.rect = new Rect(0f, 0f, 1f, 1f);
        }

        if (leftCamera != null)
        {
            leftCamera.enabled = false;
        }
    }
}
