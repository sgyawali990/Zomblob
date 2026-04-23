using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;

    // Used by Movement.cs
    public float CurrentSpeed { get; private set; }

    // World-space point the player is aiming at
    public Vector3 AimPoint { get; private set; }

    private Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleAiming();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 moveDir = camForward * v + camRight * h;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            rb.MovePosition(rb.position + moveDir.normalized * speed * Time.deltaTime);
        }

        if (moveDir.sqrMagnitude > 0.001f)
        {
            CurrentSpeed = isSprinting ? 1f : 0.5f;
        }
        else
        {
            CurrentSpeed = 0f;
        }
    }

    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 aimPoint = hit.point;
            aimPoint.y = transform.position.y;

            AimPoint = aimPoint;

            Vector3 lookDir = AimPoint - transform.position;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }
    }
}