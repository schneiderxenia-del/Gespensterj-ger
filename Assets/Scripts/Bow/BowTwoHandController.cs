using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Main controller for the two-handed bow interaction.
    /// Handles drawing, aiming, and releasing arrows.
    /// </summary>
    public class BowTwoHandController : TwoHandGrabInteractable
    {
        [Header("Bow Settings")]
        [SerializeField]
        [Tooltip("Maximum draw length for the bow")]
        private float maxDrawLength = 0.5f;

        [SerializeField]
        [Tooltip("Force multiplier for arrow velocity")]
        private float arrowForceMultiplier = 20f;

        [SerializeField]
        [Tooltip("Minimum draw percentage to release an arrow (0-1)")]
        [Range(0f, 1f)]
        private float minDrawToFire = 0.3f;

        [Header("References")]
        [SerializeField]
        [Tooltip("The bow string component")]
        private BowString bowString;

        [SerializeField]
        [Tooltip("Arrow spawn point (nocking point)")]
        private Transform arrowSpawnPoint;

        [SerializeField]
        [Tooltip("Arrow prefab to spawn")]
        private GameObject arrowPrefab;

        [Header("Haptics")]
        [SerializeField]
        [Tooltip("Enable haptic feedback")]
        private bool enableHaptics = true;

        [SerializeField]
        [Tooltip("Haptic intensity when drawing (0-1)")]
        [Range(0f, 1f)]
        private float drawHapticIntensity = 0.3f;

        [SerializeField]
        [Tooltip("Haptic duration when releasing")]
        private float releaseHapticDuration = 0.2f;

        private GameObject currentArrow;
        private float currentDrawAmount = 0f;
        private bool isDrawing = false;

        public float CurrentDrawAmount => currentDrawAmount;
        public bool IsDrawing => isDrawing;

        protected override void Awake()
        {
            base.Awake();

            // Auto-find components if not assigned
            if (bowString == null)
                bowString = GetComponentInChildren<BowString>();

            if (arrowSpawnPoint == null)
            {
                GameObject spawnPoint = new GameObject("ArrowSpawnPoint");
                spawnPoint.transform.SetParent(transform);
                spawnPoint.transform.localPosition = Vector3.zero;
                spawnPoint.transform.localRotation = Quaternion.identity;
                arrowSpawnPoint = spawnPoint.transform;
            }
        }

        private void Update()
        {
            if (isTwoHandGrabbed)
            {
                UpdateBowDraw();
                ApplyDrawHaptics();
            }
            else if (isDrawing)
            {
                // Release the bow if second hand is lost
                ReleaseBow();
            }
        }

        protected override void OnSecondHandGrabStart()
        {
            base.OnSecondHandGrabStart();
            StartDrawing();
        }

        protected override void OnSecondHandGrabEnd()
        {
            if (isDrawing)
            {
                ReleaseBow();
            }
            base.OnSecondHandGrabEnd();
        }

        private void StartDrawing()
        {
            isDrawing = true;
            
            // Spawn arrow if we have a prefab
            if (arrowPrefab != null && currentArrow == null)
            {
                currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
                currentArrow.transform.SetParent(arrowSpawnPoint);
                
                // Disable arrow physics while nocked
                Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
                if (arrowRb != null)
                {
                    arrowRb.isKinematic = true;
                }
            }
        }

        private void UpdateBowDraw()
        {
            if (!isTwoHandGrabbed || secondHandInteractor == null)
                return;

            // Calculate draw amount based on hand distance
            float handsDistance = GetHandsDistance();
            currentDrawAmount = Mathf.Clamp01(handsDistance / maxDrawLength);

            // Update bow string visual
            if (bowString != null)
            {
                Vector3 secondHandPos = GetSecondHandPosition();
                bowString.UpdateString(secondHandPos, currentDrawAmount);
            }

            // Update arrow position if it exists
            if (currentArrow != null)
            {
                // Arrow follows the draw position
                Vector3 drawPosition = Vector3.Lerp(
                    arrowSpawnPoint.position,
                    GetSecondHandPosition(),
                    currentDrawAmount
                );
                currentArrow.transform.position = drawPosition;
            }
        }

        private void ReleaseBow()
        {
            if (!isDrawing)
                return;

            // Only fire if drawn past minimum threshold
            if (currentDrawAmount >= minDrawToFire && currentArrow != null)
            {
                FireArrow();
            }
            else if (currentArrow != null)
            {
                // Not drawn enough, just destroy the arrow
                Destroy(currentArrow);
            }

            // Apply release haptics
            if (enableHaptics)
            {
                ApplyReleaseHaptics();
            }

            // Reset state
            currentArrow = null;
            currentDrawAmount = 0f;
            isDrawing = false;

            // Reset bow string
            if (bowString != null)
            {
                bowString.ResetString();
            }
        }

        private void FireArrow()
        {
            if (currentArrow == null)
                return;

            // Detach arrow from bow
            currentArrow.transform.SetParent(null);

            // Enable physics
            Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;

                // Calculate launch velocity
                Vector3 launchDirection = arrowSpawnPoint.forward;
                float launchForce = currentDrawAmount * arrowForceMultiplier;
                arrowRb.velocity = launchDirection * launchForce;

                // Apply slight angular velocity for realism
                arrowRb.angularVelocity = Vector3.zero;
            }

            // Notify arrow component if it exists
            Arrow arrowComponent = currentArrow.GetComponent<Arrow>();
            if (arrowComponent != null)
            {
                arrowComponent.OnLaunched(currentDrawAmount);
            }
        }

        private void ApplyDrawHaptics()
        {
            if (!enableHaptics || !isTwoHandGrabbed)
                return;

            // Apply continuous haptics to drawing hand based on draw amount
            float intensity = currentDrawAmount * drawHapticIntensity;
            if (intensity > 0.01f)
            {
                HapticsHelper.SendHapticImpulse(secondHandInteractor, intensity, 0.1f);
            }
        }

        private void ApplyReleaseHaptics()
        {
            if (!enableHaptics)
                return;

            // Strong pulse on release for both hands
            float releaseIntensity = Mathf.Clamp01(currentDrawAmount);
            
            HapticsHelper.SendHapticImpulse(firstInteractorSelecting, releaseIntensity, releaseHapticDuration);
            if (secondHandInteractor != null)
            {
                HapticsHelper.SendHapticImpulse(secondHandInteractor, releaseIntensity, releaseHapticDuration);
            }
        }

        // Gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            if (arrowSpawnPoint != null)
            {
                // Draw spawn point
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(arrowSpawnPoint.position, 0.02f);
                Gizmos.DrawRay(arrowSpawnPoint.position, arrowSpawnPoint.forward * 0.3f);

                // Draw max draw length
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(arrowSpawnPoint.position - arrowSpawnPoint.forward * maxDrawLength, 0.02f);
            }
        }
    }
}
