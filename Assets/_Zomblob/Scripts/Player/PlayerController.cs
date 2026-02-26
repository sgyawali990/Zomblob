using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float aimDistance = 10f;

    public LineRenderer line;
    public Vector3 AimDirection { get; private set; }

    void Start()
    {
        // Top-down aiming uses visible cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        line.positionCount = 2;
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Camera-relative movement
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Flatten (ignore camera tilt)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // Aim using mouse position in world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;

            AimDirection = (lookPoint - transform.position).normalized;

            if (AimDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(AimDirection);
            }

            // Aim line
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 end = origin + AimDirection * aimDistance;

            line.SetPosition(0, origin);
            line.SetPosition(1, end);
        }
    }
}