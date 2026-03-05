using UnityEngine;

public class BatController : MonoBehaviour
{
    public float swingTime = 0.30f;
    public float swingAngle = 80f;
    public float damage = 20f;

    private bool swinging = false;
    private bool hitSomething = false;

    private float timer = 0f;

    private Quaternion startRot;
    private Quaternion targetRot;

    private bool swingLeft = true;

    void Start()
    {
        // capture correct rotation AFTER parenting
        startRot = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !swinging)
        {
            StartSwing();
        }

        if (swinging)
        {
            timer += Time.deltaTime;
            float halfTime = swingTime / 2f;

            if (timer <= halfTime)
            {
                // Swing Forward
                float t = timer / halfTime;
                t = Mathf.SmoothStep(0f, 1f, t);
                transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            }
            else if (timer <= swingTime)
            {
                // Return Swing
                float t = (timer - halfTime) / halfTime;
                transform.localRotation = Quaternion.Lerp(targetRot, startRot, t);
            }
            else
            {
                swinging = false;
                timer = 0f;
                transform.localRotation = startRot;
            }
        }
    }

    void StartSwing()
    {
        swinging = true;
        timer = 0f;
        hitSomething = false;

        float dir = swingLeft ? -1f : 1f;

        targetRot = startRot * Quaternion.Euler(-30f, dir * swingAngle, 20f);

        swingLeft = !swingLeft;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!swinging || hitSomething)
            return;

        IDamageable dmg = other.GetComponent<IDamageable>();

        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            hitSomething = true;

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position);
                knockDir.y = 0;
                knockDir.Normalize();

                rb.AddForce(knockDir * 6f, ForceMode.Impulse);
            }
        }
    }
}