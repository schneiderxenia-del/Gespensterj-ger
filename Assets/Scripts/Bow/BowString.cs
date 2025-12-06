using UnityEngine;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Manages the bow string visualization.
    /// Updates the string position based on draw amount.
    /// </summary>
    public class BowString : MonoBehaviour
    {
        [Header("String Settings")]
        [SerializeField]
        [Tooltip("LineRenderer for the string visual")]
        private LineRenderer stringRenderer;

        [SerializeField]
        [Tooltip("Top attachment point of the string")]
        private Transform topAttachPoint;

        [SerializeField]
        [Tooltip("Bottom attachment point of the string")]
        private Transform bottomAttachPoint;

        [SerializeField]
        [Tooltip("Center point where the string is pulled (nocking point)")]
        private Transform centerPoint;

        [SerializeField]
        [Tooltip("Maximum distance the string can be pulled back")]
        private float maxPullDistance = 0.5f;

        [SerializeField]
        [Tooltip("Smoothing factor for string movement (0-1)")]
        [Range(0f, 1f)]
        private float smoothing = 0.1f;

        private Vector3 restPosition;
        private Vector3 currentPullPosition;
        private Vector3 targetPullPosition;

        private void Awake()
        {
            // Auto-setup if not configured
            if (stringRenderer == null)
            {
                stringRenderer = GetComponent<LineRenderer>();
                if (stringRenderer == null)
                {
                    stringRenderer = gameObject.AddComponent<LineRenderer>();
                    SetupLineRenderer();
                }
            }

            // Store rest position
            if (centerPoint != null)
            {
                restPosition = centerPoint.localPosition;
                currentPullPosition = centerPoint.position;
                targetPullPosition = currentPullPosition;
            }
        }

        private void SetupLineRenderer()
        {
            stringRenderer.positionCount = 3;
            stringRenderer.startWidth = 0.005f;
            stringRenderer.endWidth = 0.005f;
            stringRenderer.material = new Material(Shader.Find("Sprites/Default"));
            stringRenderer.startColor = Color.black;
            stringRenderer.endColor = Color.black;
            stringRenderer.useWorldSpace = true;
        }

        private void LateUpdate()
        {
            // Smooth the string position
            if (smoothing > 0f)
            {
                currentPullPosition = Vector3.Lerp(currentPullPosition, targetPullPosition, 1f - smoothing);
            }
            else
            {
                currentPullPosition = targetPullPosition;
            }

            UpdateStringVisual();
        }

        /// <summary>
        /// Update the string position based on draw hand position
        /// </summary>
        public void UpdateString(Vector3 drawHandPosition, float drawAmount)
        {
            if (centerPoint == null)
                return;

            // Calculate target pull position
            // The string is pulled back along the bow's local forward axis
            Vector3 pullDirection = -transform.forward;
            float pullDistance = drawAmount * maxPullDistance;
            
            // Project the hand position onto the pull direction for more natural feel
            Vector3 centerToHand = drawHandPosition - centerPoint.position;
            float projectedDistance = Vector3.Dot(centerToHand, pullDirection);
            projectedDistance = Mathf.Clamp(projectedDistance, 0f, maxPullDistance);

            targetPullPosition = centerPoint.position + pullDirection * projectedDistance;
        }

        /// <summary>
        /// Reset string to rest position
        /// </summary>
        public void ResetString()
        {
            if (centerPoint != null)
            {
                targetPullPosition = centerPoint.position;
            }
        }

        /// <summary>
        /// Update the LineRenderer to show the string
        /// </summary>
        private void UpdateStringVisual()
        {
            if (stringRenderer == null || topAttachPoint == null || bottomAttachPoint == null)
                return;

            // Set the three points of the string
            stringRenderer.SetPosition(0, topAttachPoint.position);
            stringRenderer.SetPosition(1, currentPullPosition);
            stringRenderer.SetPosition(2, bottomAttachPoint.position);
        }

        /// <summary>
        /// Get the current draw distance from rest position
        /// </summary>
        public float GetCurrentDrawDistance()
        {
            if (centerPoint == null)
                return 0f;

            return Vector3.Distance(centerPoint.position, currentPullPosition);
        }

        // Gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            if (centerPoint == null)
                return;

            // Draw rest position
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerPoint.position, 0.02f);

            // Draw max pull position
            Gizmos.color = Color.red;
            Vector3 maxPullPos = centerPoint.position - transform.forward * maxPullDistance;
            Gizmos.DrawWireSphere(maxPullPos, 0.02f);
            Gizmos.DrawLine(centerPoint.position, maxPullPos);

            // Draw attachment points
            if (topAttachPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(topAttachPoint.position, 0.03f);
            }

            if (bottomAttachPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(bottomAttachPoint.position, 0.03f);
            }
        }
    }
}
