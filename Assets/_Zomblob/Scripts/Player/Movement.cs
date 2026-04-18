using UnityEngine;

public class Movement : MonoBehaviour
{
    private Animator mAnimator;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 28f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D); 
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && isWalking;

            float targetSpeed = 0f;
            if (isRunning)
                targetSpeed = runSpeed;
            else if (isWalking)
                targetSpeed = moveSpeed;
            
            float currentSpeed = mAnimator.GetFloat("Speed");
            float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
            mAnimator.SetFloat("Speed", newSpeed);
        }
    }
}
