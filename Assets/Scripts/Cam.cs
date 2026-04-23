using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5f;
    Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    public void MoveCamera(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}