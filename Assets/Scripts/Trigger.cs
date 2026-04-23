using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform cameraTarget;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>()
                .MoveCamera(cameraTarget.position);
        }
    }
}