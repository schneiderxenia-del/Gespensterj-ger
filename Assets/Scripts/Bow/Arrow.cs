using UnityEngine;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Kontrolliert das Pfeil-Verhalten inklusive Physik, Kollision und Lebensdauer.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour
    {
        [Header("Arrow Settings")]
        [Tooltip("Schadenswert des Pfeils")]
        [SerializeField] private float damage = 25f;

        [Tooltip("Lebensdauer des Pfeils in Sekunden")]
        [SerializeField] private float lifetime = 10f;

        [Tooltip("Minimum-Geschwindigkeit für Schaden")]
        [SerializeField] private float minDamageVelocity = 5f;

        [Header("Physics")]
        [Tooltip("Masse des Pfeils")]
        [SerializeField] private float arrowMass = 0.05f;

        [Tooltip("Luftwiderstand")]
        [SerializeField] private float drag = 0.1f;

        [Tooltip("Schwerkraft-Multiplikator")]
        [SerializeField] private float gravityMultiplier = 1.5f;

        [Header("Visual Settings")]
        [Tooltip("Pfeilspitze (für Hit-Detection)")]
        [SerializeField] private Transform arrowTip;

        [Tooltip("Trail-Effekt")]
        [SerializeField] private TrailRenderer trailRenderer;

        private Rigidbody rb;
        private Collider arrowCollider;
        private bool isAttachedToBow = false;
        private bool hasLaunched = false;
        private bool hasHit = false;
        private Transform parentBow;
        private Vector3 lastPosition;
        private Vector3 lastVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            arrowCollider = GetComponent<Collider>();

            // Setup Rigidbody
            rb.mass = arrowMass;
            rb.drag = drag;
            rb.useGravity = false; // Wir verwenden custom gravity
            rb.isKinematic = true;

            // Deaktiviere Kollision bis Launch
            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;
            }

            if (arrowTip == null)
            {
                arrowTip = transform;
            }
        }

        /// <summary>
        /// Befestigt den Pfeil am Bogen
        /// </summary>
        public void AttachToBow(Transform bowAttachPoint)
        {
            isAttachedToBow = true;
            hasLaunched = false;
            hasHit = false;
            parentBow = bowAttachPoint;

            // Setze Pfeil als Child des Bogens
            transform.SetParent(bowAttachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Deaktiviere Physics
            rb.isKinematic = true;
            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;
            }

            // Deaktiviere Trail
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }
        }

        /// <summary>
        /// Löst den Pfeil vom Bogen
        /// </summary>
        public void DetachFromBow()
        {
            isAttachedToBow = false;
            transform.SetParent(null);
        }

        /// <summary>
        /// Schießt den Pfeil ab
        /// </summary>
        public void Launch(Vector3 direction, float velocity)
        {
            if (hasLaunched)
            {
                return;
            }

            hasLaunched = true;
            isAttachedToBow = false;

            // Aktiviere Physics
            rb.isKinematic = false;
            rb.useGravity = false; // Custom gravity
            
            if (arrowCollider != null)
            {
                arrowCollider.enabled = true;
            }

            // Setze Geschwindigkeit
            rb.velocity = direction.normalized * velocity;
            lastVelocity = rb.velocity;

            // Aktiviere Trail
            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
                trailRenderer.Clear();
            }

            // Zerstöre nach Lebensdauer
            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            if (!hasLaunched || hasHit)
            {
                return;
            }

            // Custom Gravity
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

            // Rotiere Pfeil in Flugrichtung
            if (rb.velocity.magnitude > 0.1f)
            {
                transform.forward = rb.velocity.normalized;
                lastVelocity = rb.velocity;
            }

            lastPosition = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!hasLaunched || hasHit)
            {
                return;
            }

            hasHit = true;

            // Berechne Aufprallgeschwindigkeit
            float impactVelocity = lastVelocity.magnitude;

            // Prüfe ob Schaden verursacht werden soll
            if (impactVelocity >= minDamageVelocity)
            {
                // Versuche Schaden zu verursachen
                var damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }

            // Stoppe Pfeil
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Deaktiviere Trail
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            // Befestige Pfeil am getroffenen Objekt
            if (collision.rigidbody != null)
            {
                transform.SetParent(collision.transform);
            }
            else if (collision.collider != null)
            {
                transform.SetParent(collision.transform);
            }

            // Deaktiviere Collider um weitere Kollisionen zu vermeiden
            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (arrowTip != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(arrowTip.position, 0.01f);
                
                if (hasLaunched && !hasHit)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(arrowTip.position, lastVelocity.normalized * 0.2f);
                }
            }
        }
    }

    /// <summary>
    /// Interface für Objekte, die Schaden nehmen können
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
