using UnityEngine;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Kontrolliert die Bogensehne und deren visuelle Darstellung beim Spannen.
    /// Verwaltet die Sehnen-Geometrie und Position während des Ziehens.
    /// </summary>
    public class BowString : MonoBehaviour
    {
        [Header("String Points")]
        [Tooltip("Oberer Befestigungspunkt der Sehne am Bogen")]
        [SerializeField] private Transform topStringPoint;

        [Tooltip("Unterer Befestigungspunkt der Sehne am Bogen")]
        [SerializeField] private Transform bottomStringPoint;

        [Tooltip("Mittlerer Punkt der Sehne (wird beim Ziehen bewegt)")]
        [SerializeField] private Transform middleStringPoint;

        [Header("Draw Settings")]
        [Tooltip("Maximale Rückzugsdistanz der Sehne in Metern")]
        public float maxDrawDistance = 0.5f;

        [Tooltip("Anfangsposition des mittleren Punkts (relativ zum Bogen)")]
        [SerializeField] private Vector3 restPosition = Vector3.zero;

        [Header("Visual Settings")]
        [Tooltip("LineRenderer für die Sehnen-Visualisierung")]
        [SerializeField] private LineRenderer lineRenderer;

        [Tooltip("Breite der Sehne")]
        [SerializeField] private float stringWidth = 0.005f;

        [Tooltip("Material für die Sehne")]
        [SerializeField] private Material stringMaterial;

        private Vector3 currentDrawPosition;
        private bool isInitialized = false;

        private void Awake()
        {
            InitializeString();
        }

        private void Start()
        {
            if (!isInitialized)
            {
                InitializeString();
            }
        }

        private void InitializeString()
        {
            if (isInitialized)
            {
                return;
            }

            // Setup LineRenderer
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }

            lineRenderer.positionCount = 3;
            lineRenderer.startWidth = stringWidth;
            lineRenderer.endWidth = stringWidth;
            lineRenderer.useWorldSpace = true;

            if (stringMaterial != null)
            {
                lineRenderer.material = stringMaterial;
            }

            // Setze Anfangsposition
            if (middleStringPoint != null)
            {
                restPosition = middleStringPoint.localPosition;
                currentDrawPosition = middleStringPoint.position;
            }

            UpdateStringVisual();
            isInitialized = true;
        }

        /// <summary>
        /// Aktualisiert die Sehnen-Position basierend auf der Zugstärke
        /// </summary>
        /// <param name="drawStrength">Zugstärke von 0 bis 1</param>
        /// <param name="handPosition">Position der ziehenden Hand</param>
        public void UpdateDraw(float drawStrength, Vector3 handPosition)
        {
            if (!isInitialized)
            {
                InitializeString();
            }

            // Berechne die Zielposition basierend auf Hand-Position
            Vector3 targetPosition = CalculateDrawPosition(handPosition, drawStrength);
            
            // Smooth damp für flüssigere Bewegung (optional)
            currentDrawPosition = Vector3.Lerp(currentDrawPosition, targetPosition, Time.deltaTime * 20f);

            if (middleStringPoint != null)
            {
                middleStringPoint.position = currentDrawPosition;
            }

            UpdateStringVisual();
        }

        /// <summary>
        /// Setzt die Sehne in die Ruheposition zurück
        /// </summary>
        public void ResetDraw()
        {
            if (!isInitialized)
            {
                InitializeString();
            }

            if (middleStringPoint != null)
            {
                middleStringPoint.localPosition = restPosition;
                currentDrawPosition = middleStringPoint.position;
            }

            UpdateStringVisual();
        }

        private Vector3 CalculateDrawPosition(Vector3 handPosition, float drawStrength)
        {
            if (middleStringPoint == null)
            {
                return handPosition;
            }

            // Berechne die Richtung vom mittleren Punkt zur Hand
            Vector3 stringCenter = (topStringPoint.position + bottomStringPoint.position) / 2f;
            Vector3 toHand = handPosition - stringCenter;

            // Projiziere auf die Rückwärts-Achse des Bogens
            Vector3 drawDirection = -transform.forward;
            float drawDistance = Vector3.Dot(toHand, drawDirection);

            // Begrenze die Distanz
            drawDistance = Mathf.Clamp(drawDistance, 0f, maxDrawDistance);

            // Berechne finale Position
            return stringCenter + drawDirection * drawDistance;
        }

        private void UpdateStringVisual()
        {
            if (lineRenderer == null || topStringPoint == null || bottomStringPoint == null)
            {
                return;
            }

            // Setze die drei Punkte der Sehne
            lineRenderer.SetPosition(0, topStringPoint.position);
            lineRenderer.SetPosition(1, currentDrawPosition);
            lineRenderer.SetPosition(2, bottomStringPoint.position);
        }

        private void LateUpdate()
        {
            // Aktualisiere die Sehnen-Visual jedes Frame
            UpdateStringVisual();
        }

        private void OnDrawGizmos()
        {
            if (topStringPoint != null && bottomStringPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(topStringPoint.position, 0.01f);
                Gizmos.DrawWireSphere(bottomStringPoint.position, 0.01f);

                if (middleStringPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(middleStringPoint.position, 0.015f);
                    
                    // Zeichne Sehne
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(topStringPoint.position, middleStringPoint.position);
                    Gizmos.DrawLine(middleStringPoint.position, bottomStringPoint.position);
                }

                // Zeichne maximale Zugdistanz
                Vector3 center = (topStringPoint.position + bottomStringPoint.position) / 2f;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(center - transform.forward * maxDrawDistance, 0.02f);
            }
        }
    }
}
