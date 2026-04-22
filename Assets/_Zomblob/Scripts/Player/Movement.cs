using UnityEngine;

public class Movement : MonoBehaviour
{
    private Animator mAnimator;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 7f; // 28 was likely too high for top-down!
    [SerializeField] private float acceleration = 10f;

    // Cache hashes for performance (better than strings)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsAimingHash = Animator.StringToHash("isAiming");

    void Start()
    {
        mAnimator = GetComponentInChildren<Animator>();

        if (mAnimator == null)
        {
            Debug.LogError("Animator NOT FOUND");
        }
    }

    void Update()
    {
        if (mAnimator == null) return;

        HandleMovementAnimations();
        HandleUpperBodyAnimations();
    }

    private void HandleMovementAnimations()
    {
        // Check for any movement key
        bool hasInput = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && hasInput;

        // Determine target speed value for animator
        float targetSpeed = 0f;
        if (isRunning) targetSpeed = runSpeed;
        else if (hasInput) targetSpeed = walkSpeed;

        // Smoothly lerp the 'Speed' parameter
        float currentSpeed = mAnimator.GetFloat(SpeedHash);
        float smoothedSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        mAnimator.SetFloat(SpeedHash, smoothedSpeed);
    }

    private void HandleUpperBodyAnimations()
    {
        // Right-Click (Hold) triggers the Aim pose in your UpperBody layer
        bool isAiming = Input.GetMouseButton(1);
        mAnimator.SetBool(IsAimingHash, isAiming);
    }
}