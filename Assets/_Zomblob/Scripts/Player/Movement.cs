using UnityEngine;

public class Movement : MonoBehaviour
{
    private Animator mAnimator;
    private PlayerController controller;

    [Header("Animation Smoothing")]
    [SerializeField] private float acceleration = 10f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsAimingHash = Animator.StringToHash("isAiming");

    void Start()
    {
        mAnimator = GetComponentInChildren<Animator>();
        controller = GetComponentInParent<PlayerController>();

        if (mAnimator == null)
        {
            Debug.LogError($"[Movement] Animator not found!");
        }

        if (controller == null)
        {
            Debug.LogError($"[Movement] PlayerController not found!");
        }
    }

    void Update()
    {
        if (mAnimator == null || controller == null) return;

        HandleMovementAnimations();
        HandleUpperBodyAnimations();
    }

    private void HandleMovementAnimations()
    {
        float targetValue = controller.CurrentSpeed;

        float current = mAnimator.GetFloat(SpeedHash);
        float smoothed = Mathf.Lerp(current, targetValue, Time.deltaTime * acceleration);

        mAnimator.SetFloat(SpeedHash, smoothed);
    }

    private void HandleUpperBodyAnimations()
    {
        bool isAiming = Input.GetMouseButton(1);
        mAnimator.SetBool(IsAimingHash, isAiming);
    }
}