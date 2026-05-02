using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingTime = 0.35f;
    public float swingAngle = 95f;
    public float verticalOffset = 30f;

    [Header("Upgraded Feel")]
    [SerializeField] private float idleBobAmount = 5f;
    [SerializeField] private float idleBobSpeed = 2f;
    [SerializeField] private float hitRadius = 2.0f; 
    [SerializeField] private LayerMask enemyLayer;   

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] swingSounds;
    [SerializeField] private AudioClip[] hitSounds;

    [Header("Damage")]
    public float damage = 20f;

    private bool swinging;
    private bool hitSomething;
    private bool canHit;
    private float timer;

    private Quaternion neutralRot;
    private Quaternion startArc;
    private Quaternion endArc;

    private bool swingLeft = true;

    void Start()
    {
        neutralRot = transform.localRotation;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(Transform root)
    {
        // Placeholder
    }

    AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;

        return clips[Random.Range(0, clips.Length)];
    }

    void Update()
    {
        // IDLE MOTION
        if (!swinging)
        {
            float idle = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
            transform.localRotation = neutralRot * Quaternion.Euler(0, idle, 0);

            if (Input.GetMouseButtonDown(0))
            {
                StartSwing();
            }
            return;
        }

        // SWING LOGIC
        timer += Time.deltaTime;
        float t = timer / swingTime;

        if (t <= 1f)
        {
            // Strong acceleration curve
            float curve = 1 - Mathf.Pow(1 - t, 3f);

            transform.localRotation = Quaternion.Slerp(startArc, endArc, curve);

            bool inWindow = (t > 0.35f && t < 0.75f);
            canHit = inWindow;

            if (inWindow && !hitSomething)
            {
                TryAoEHit();
            }
        }
        else
        {
            EndSwing();
        }
    }

    void StartSwing()
    {
        swinging = true;
        timer = 0f;
        hitSomething = false;
        canHit = false;

        if (audioSource)
        {
            AudioClip clip = GetRandomClip(swingSounds);
            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        float dir = swingLeft ? 1f : -1f;

        startArc = neutralRot * Quaternion.Euler(-verticalOffset, -dir * swingAngle, 0);
        endArc = neutralRot * Quaternion.Euler(verticalOffset, dir * swingAngle, 0);

        swingLeft = !swingLeft;
    }

    void EndSwing()
    {
        swinging = false;
        canHit = false;

        transform.localRotation = neutralRot;
    }

    void TryAoEHit()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.parent.position, 
            hitRadius,
            enemyLayer
        );

        if (hits.Length == 0) return;

        Transform player = transform.parent;

        Collider bestTarget = null;
        float bestScore = -999f;

        foreach (var col in hits)
        {
            Vector3 toEnemy = (col.transform.position - player.position).normalized;

            float dot = Vector3.Dot(player.forward, toEnemy);

            if (dot < 0.3f) continue;

            float dist = Vector3.Distance(player.position, col.transform.position);
            float score = dot * 2f - dist * 0.5f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = col;
            }
        }

        if (bestTarget != null && bestTarget.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
            hitSomething = true;

            if (audioSource)
            {
                AudioClip clip = GetRandomClip(hitSounds);
                if (clip != null)
                    audioSource.PlayOneShot(clip);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!swinging || hitSomething || !canHit)
            return;

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
            hitSomething = true;

            if (audioSource)
            {
                AudioClip clip = GetRandomClip(hitSounds);
                if (clip != null)
                    audioSource.PlayOneShot(clip);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}