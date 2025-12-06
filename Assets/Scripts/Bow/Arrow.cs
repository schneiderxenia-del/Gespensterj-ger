using UnityEngine;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Arrow projectile component.
    /// Handles arrow physics, collision, and lifetime.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour
    {
        [Header("Arrow Settings")]
        [SerializeField]
        [Tooltip("Damage dealt on hit")]
        private float damage = 10f;

        [SerializeField]
        [Tooltip("How long until the arrow is destroyed after being launched (seconds)")]
        private float lifetime = 10f;

        [SerializeField]
        [Tooltip("Should the arrow stick to surfaces on impact?")]
        private bool stickOnImpact = true;

        [SerializeField]
        [Tooltip("Minimum velocity to stick (m/s)")]
        private float minStickVelocity = 2f;

        [Header("Physics")]
        [SerializeField]
        [Tooltip("Drag multiplier when arrow is in flight")]
        private float airDrag = 0.01f;

        [SerializeField]
        [Tooltip("Should the arrow align with velocity direction?")]
        private bool alignWithVelocity = true;

        [SerializeField]
        [Tooltip("Rotation speed when aligning with velocity")]
        private float alignmentSpeed = 10f;

        private Rigidbody rb;
        private bool hasLaunched = false;
        private bool hasStuck = false;
        private float launchTime;
        private float drawAmountOnLaunch;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            // Configure rigidbody for arrow physics
            rb.useGravity = true;
            rb.drag = airDrag;
            rb.angularDrag = 0.05f;
            rb.mass = 0.05f; // 50 grams
        }

        private void FixedUpdate()
        {
            if (hasLaunched && !hasStuck && alignWithVelocity)
            {
                AlignWithVelocity();
            }
        }

        /// <summary>
        /// Called when the arrow is launched from the bow
        /// </summary>
        public void OnLaunched(float drawAmount)
        {
            hasLaunched = true;
            launchTime = Time.time;
            drawAmountOnLaunch = drawAmount;

            // Start lifetime countdown
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Align the arrow with its velocity direction for realistic flight
        /// </summary>
        private void AlignWithVelocity()
        {
            if (rb.velocity.magnitude < 0.1f)
                return;

            Vector3 velocityDirection = rb.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(velocityDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, alignmentSpeed * Time.fixedDeltaTime));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!hasLaunched || hasStuck)
                return;

            float impactVelocity = rb.velocity.magnitude;

            // Check if we should stick
            if (stickOnImpact && impactVelocity >= minStickVelocity)
            {
                StickToSurface(collision);
            }

            // Deal damage if the object can receive damage
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float scaledDamage = damage * drawAmountOnLaunch;
                damageable.TakeDamage(scaledDamage);
            }

            // Alternate: Try to find health component (common pattern)
            var health = collision.gameObject.GetComponent<PlayerHitbox>();
            if (health != null)
            {
                // Could integrate with existing health system
                Debug.Log($"Arrow hit {collision.gameObject.name} for {damage * drawAmountOnLaunch} damage");
            }
        }

        /// <summary>
        /// Make the arrow stick to the surface it hit
        /// </summary>
        private void StickToSurface(Collision collision)
        {
            hasStuck = true;

            // Stop physics
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Parent to the hit object so it moves with it
            Transform hitTransform = collision.transform;
            transform.SetParent(hitTransform);

            // Get the contact point and normal
            ContactPoint contact = collision.GetContact(0);
            
            // Position arrow at impact point slightly embedded
            transform.position = contact.point + contact.normal * -0.05f;
            
            // Orient arrow based on entry angle
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);

            // Disable collider to prevent further collisions
            Collider arrowCollider = GetComponent<Collider>();
            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;
            }
        }

        /// <summary>
        /// Get the flight time of this arrow
        /// </summary>
        public float GetFlightTime()
        {
            if (!hasLaunched)
                return 0f;
            return Time.time - launchTime;
        }

        /// <summary>
        /// Check if the arrow has stuck to a surface
        /// </summary>
        public bool IsStuck()
        {
            return hasStuck;
        }

        // Gizmos for debugging
        private void OnDrawGizmos()
        {
            if (hasLaunched && !hasStuck)
            {
                // Draw velocity vector
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, rb.velocity.normalized * 0.5f);
            }
        }
    }

    /// <summary>
    /// Interface for objects that can take damage from arrows
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
