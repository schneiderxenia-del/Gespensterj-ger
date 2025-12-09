using UnityEngine;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/GhostAI")]
[RequireComponent(typeof(Collider))]
public class GhostAI : MonoBehaviour
{
    [Header("Target (Spieler)")]
    public Transform target;                  // Spieler (meist HMD/Kamera)

    [Header("Bewegung")]
    public float moveSpeed = 1.2f;            // Laufgeschwindigkeit
    public float turnSpeed = 6f;              // Wie schnell der Geist dreht
    public float stopDistance = 0.8f;         // Abstand, bei dem er stehen bleibt
    public float killDistance = 7f;           // Abstand, bei dem der Spieler stirbt

    [Header("Hover über Boden")]
    public float hoverHeight = 0.30f;         // Standardhöhe über Boden
    public float hoverAmplitude = 0.05f;      // Auf/Ab Bewegung
    public float hoverFrequency = 2f;         // Geschwindigkeit der Hover-Bewegung

    [Header("Boden-Erkennung")]
    public LayerMask groundMask = ~0;         // Welche Layer als Boden gelten
    public float groundRayUp = 1.0f;          
    public float groundRayDown = 5.0f;        // Reichweite der Bodenprüfung
    public float groundProbeRadius = 0.10f;   // Radius für Spherecast

    [Header("Lebenszeit / Treffer")]
    public int health = 1;                    // Trefferpunkte
    public float maxLifetime = 60f;           // Geist zerstört sich nach Zeit

    float _life;
    float _pivotToBottom = 0f;                // Offset zwischen Pivot und Modellboden

        public ParticleSystem sparkleParticles;
    void Awake()
    {
        // Geist soll schweben → Rigidbody deaktivieren
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void OnEnable()
    {
        _life = 0f;

        // Spielerautomatisch suchen, falls nicht gesetzt
        if (!target && Camera.main) 
            target = Camera.main.transform;

        // Abstand zum Modellboden bestimmen
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            _pivotToBottom = transform.position.y - rend.bounds.min.y;
            if (_pivotToBottom < 0f) _pivotToBottom = 0f;
        }

        // Geist korrekt über Boden positionieren
        if (TryGetGroundY(transform.position, out float gy))
        {
            var p = transform.position;
            p.y = gy + _pivotToBottom + hoverHeight;
            transform.position = p;
        }
    }

    void Update()
    {
        // Selbstzerstörung nach Ablauf der Lebenszeit
        _life += Time.deltaTime;
        if (_life > maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (!target) return;

        // Richtung zum Spieler
        Vector3 to = target.position - transform.position;
        Vector3 flat = new Vector3(to.x, 0f, to.z);
        float dist = flat.magnitude;

        // Zum Spieler drehen
        if (flat.sqrMagnitude > 0.001f)
        {
            var look = Quaternion.LookRotation(flat.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // Auf Spieler zulaufen
        if (dist > stopDistance)
        {
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }

        // Hover-Bewegung
        if (TryGetGroundY(transform.position, out float gy))
        {
            float baseY = gy + _pivotToBottom + hoverHeight;
            float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            var p = transform.position;
            p.y = baseY + hover;
            transform.position = p;
        }

        // Spieler töten, wenn zu nah
        if (dist <= killDistance)
        {
            GameManager.Instance?.PlayerDied();
        }
    }


private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Arrow"))
    {
        health--;

        if (health <= 0)
        {
            // Renderer ausschalten
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            // Partikelsystem starten
            if (sparkleParticles != null)
                sparkleParticles.Play();

            // Geist zerstören nach Partikeleffekt
            Destroy(gameObject, sparkleParticles != null ? sparkleParticles.main.duration : 0.5f);
        }
    }
}




    // Bodenhöhe bestimmen
    bool TryGetGroundY(Vector3 fromPos, out float groundY)
    {
        groundY = 0f;

        Vector3 origin = fromPos + Vector3.up * groundRayUp;
        float maxDist = groundRayUp + groundRayDown;

        // Erst normaler Raycast
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDist, groundMask))
        {
            groundY = hit.point.y;
            return true;
        }

        // Falls verfehlt → Spherecast benutzen
        if (groundProbeRadius > 0f &&
            Physics.SphereCast(origin, groundProbeRadius, Vector3.down, out hit, maxDist, groundMask))
        {
            groundY = hit.point.y;
            return true;
        }

        return false;
    }



#if UNITY_EDITOR
    // Debug Anzeige in der Scene View
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
