using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float aimDistance = 10f;

    // World-space point the player is aiming at
    public Vector3 AimPoint { get; private set; }

    //moved it to here so it dosne't have to run every frame
    private Rigidbody rb;
    
    void Start()
    {
        // Top-down aiming uses visible cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        rb = GetComponent<Rigidbody>();
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.visible = !Cursor.visible;
        }
        //sprinting (hopefully))
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 moveDir = camForward * v + camRight * h;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            rb.MovePosition(rb.position + moveDir * currentSpeed * Time.deltaTime);
        }

        // Aim using mouse position in world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 aimPoint = hit.point;

            // Flatten aim point to player height
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