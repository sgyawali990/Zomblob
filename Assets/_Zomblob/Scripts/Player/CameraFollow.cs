using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Offsets")]
    public Vector3 normalOffset;
    public Vector3 zoomOffset;

    [Header("Settings")]
    public float smoothSpeed = 8f;

    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = normalOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool isAiming = Input.GetMouseButton(1);
        Vector3 targetOffset = isAiming ? zoomOffset : normalOffset;

        if (targetOffset.z > -1f) targetOffset.z = -1f;
        if (targetOffset.y < 2f) targetOffset.y = 2f;

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);
        transform.position = target.position + currentOffset;

        Vector3 lookTarget = target.position + Vector3.up * 1.2f;
        transform.LookAt(lookTarget);
    }
}