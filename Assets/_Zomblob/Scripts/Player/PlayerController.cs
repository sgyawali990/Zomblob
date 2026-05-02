using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;

    public float CurrentSpeed { get; private set; }
    public Vector3 AimPoint { get; private set; }

    private Rigidbody rb;
    private bool isMenuOpen = false;

    private Vector3 moveInput;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        SetGameplayState(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuOpen = !isMenuOpen;
            SetGameplayState(!isMenuOpen);
        }

        if (!isMenuOpen)
        {
            HandleMovementInput(); 
            HandleAiming();
        }
        else
        {
            CurrentSpeed = 0f;
            moveInput = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (isMenuOpen) return;

        float speed = isSprinting ? sprintSpeed : moveSpeed;

        if (moveInput.sqrMagnitude > 0.001f)
        {
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SetGameplayState(!isMenuOpen);
        }
    }

    private void SetGameplayState(bool playing)
    {
        if (playing)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveInput = (camForward * v + camRight * h).normalized;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        bool hasInput = h != 0f || v != 0f;
        CurrentSpeed = !hasInput ? 0f : (isSprinting ? 1f : 0.5f);
    }

    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            hitPoint.y = transform.position.y;

            AimPoint = hitPoint;
            Vector3 lookDir = AimPoint - transform.position;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime * 15f));
            }
        }
    }
}