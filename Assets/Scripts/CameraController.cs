using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, 0);
    [SerializeField] private float cameraSpeed = 5.0f;

    private Camera cam;
    private Vector3 targetPosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        targetPosition = target.transform.position + offset;
        Vector3 dir = targetPosition - this.transform.position;
        Vector3 moveVector = new Vector3(dir.x * cameraSpeed * Time.deltaTime, dir.y * cameraSpeed * Time.deltaTime, 0.0f);
        this.transform.Translate(moveVector);
    }
}
