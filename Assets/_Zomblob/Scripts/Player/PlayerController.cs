using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;

    public float CurrentSpeed { get; private set; }
    public Vector3 AimPoint { get; private set; }

    private Rigidbody rb;
    private bool isMenuOpen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start game with aiming active
        SetGameplayState(true);
    }

    void Update()
    {
        // Toggle the menu state on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuOpen = !isMenuOpen;
            SetGameplayState(!isMenuOpen);
        }

        if (!isMenuOpen)
        {
            HandleMovement();
            HandleAiming();
        }
        else
        {
            CurrentSpeed = 0f;
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

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        bool hasInput = h != 0f || v != 0f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : moveSpeed;

        if (hasInput)
        {
            rb.MovePosition(rb.position + moveDir.normalized * speed * Time.deltaTime);
        }

        CurrentSpeed = !hasInput ? 0f : (isSprinting ? 1f : 0.5f);
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