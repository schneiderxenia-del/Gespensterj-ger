using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GhostAI : MonoBehaviour
{
    [Header("Target (Spieler)")]
    public Transform target;                       // meist Main Camera (HMD)

    [Header("Bewegung")]
    public float moveSpeed = 1.2f;
    public float turnSpeed = 6f;
    public float stopDistance = 0.8f;
    public float killDistance = 7f;

    [Header("Hover über Boden")]
    public float hoverHeight = 0.30f;
    public float hoverAmplitude = 0.05f;
    public float hoverFrequency = 2f;

    [Header("Boden-Erkennung")]
    public LayerMask groundMask = ~0;
    public float groundRayUp = 1.0f;
    public float groundRayDown = 5.0f;
    public float groundProbeRadius = 0.10f;

    [Header("Lebenszeit / Treffer")]
    public int health = 1;
    public float maxLifetime = 60f;

    float _life;
    float _pivotToBottom = 0f;

    void Awake()
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    void OnEnable()
    {
        _life = 0f;
        if (!target && Camera.main) target = Camera.main.transform;

        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            _pivotToBottom = transform.position.y - rend.bounds.min.y;
            if (_pivotToBottom < 0f) _pivotToBottom = 0f;
        }

        if (TryGetGroundY(transform.position, out float gy))
        {
            var p = transform.position;
            p.y = gy + _pivotToBottom + hoverHeight;
            transform.position = p;
        }
    }

    void Update()
    {
        _life += Time.deltaTime;
        if (_life > maxLifetime) { Destroy(gameObject); return; }
        if (!target) return;

        Vector3 to = target.position - transform.position;
        Vector3 flat = new Vector3(to.x, 0f, to.z);
        float dist = flat.magnitude;



        if (flat.sqrMagnitude > 1e-4f)
        {
            var look = Quaternion.LookRotation(flat.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
        if (dist > stopDistance)
        {
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }

        if (TryGetGroundY(transform.position, out float gy))
        {
            float baseY = gy + _pivotToBottom + hoverHeight;
            float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            var p = transform.position;
            p.y = baseY + hover;
            transform.position = p;
        }

        if (dist <= killDistance)
        {
            GameManagerVR.Instance?.PlayerDied();
        }
    }

    // -----------------------------------------------------------
    // ÜBERARBEITET: Geist zählt Score + stirbt korrekt
    // -----------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            health -= 1;

            if (health <= 0)
            {
                // Score erhöhen
                GameManagerVR.Instance?.AddScore(1);

                Destroy(gameObject);
            }
        }
    }

    bool TryGetGroundY(Vector3 fromPos, out float groundY)
    {
        groundY = 0f;
        Vector3 origin = fromPos + Vector3.up * groundRayUp;
        float maxDist = groundRayUp + groundRayDown;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDist, groundMask, QueryTriggerInteraction.Collide))
        {
            groundY = hit.point.y;
            return true;
        }

        if (groundProbeRadius > 0f &&
            Physics.SphereCast(origin, groundProbeRadius, Vector3.down, out hit, maxDist, groundMask, QueryTriggerInteraction.Collide))
        {
            groundY = hit.point.y;
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * groundRayUp;
        float maxDist = groundRayUp + groundRayDown;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector3.down * maxDist);
        Gizmos.DrawWireSphere(origin + Vector3.down * maxDist, groundProbeRadius);
    }
#endif
}
