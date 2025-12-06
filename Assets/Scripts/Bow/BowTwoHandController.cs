using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Kontrolliert die Bogen-Mechanik mit zweihändiger Interaktion.
    /// Verwaltet das Spannen der Sehne, Pfeil-Instanziierung und Schusslogik.
    /// </summary>
    [RequireComponent(typeof(TwoHandGrabInteractable))]
    public class BowTwoHandController : MonoBehaviour
    {
        [Header("Bow Components")]
        [Tooltip("Der TwoHandGrabInteractable-Komponente des Bogens")]
        [SerializeField] private TwoHandGrabInteractable bowInteractable;

        [Tooltip("Die BowString-Komponente")]
        [SerializeField] private BowString bowString;

        [Header("Arrow Settings")]
        [Tooltip("Das Pfeil-Prefab")]
        [SerializeField] private GameObject arrowPrefab;

        [Tooltip("Die Spawn-Position des Pfeils")]
        [SerializeField] private Transform arrowSpawnPoint;

        [Tooltip("Minimale Zugstärke für einen Schuss (0-1)")]
        [SerializeField] private float minDrawStrength = 0.3f;

        [Header("Physics Settings")]
        [Tooltip("Maximale Pfeilgeschwindigkeit")]
        [SerializeField] private float maxArrowVelocity = 30f;

        [Tooltip("Multiplikator für die Pfeilgeschwindigkeit basierend auf Zugstärke")]
        [SerializeField] private float velocityMultiplier = 1f;

        [Header("Haptics")]
        [Tooltip("Haptic-Feedback beim Spannen")]
        [SerializeField] private bool useHaptics = true;

        [SerializeField] private float drawHapticIntensity = 0.1f;
        [SerializeField] private float releaseHapticIntensity = 0.5f;
        [SerializeField] private float hapticDuration = 0.1f;

        private Arrow currentArrow;
        private bool isDrawing = false;
        private float currentDrawStrength = 0f;
        private XRBaseInteractor primaryHand;

        private void Awake()
        {
            if (bowInteractable == null)
            {
                bowInteractable = GetComponent<TwoHandGrabInteractable>();
            }
        }

        private void OnEnable()
        {
            bowInteractable.selectEntered.AddListener(OnBowGrabbed);
            bowInteractable.selectExited.AddListener(OnBowReleased);
            bowInteractable.OnSecondHandGrabbed += OnSecondHandGrabbed;
            bowInteractable.OnSecondHandReleased += OnSecondHandReleased;
        }

        private void OnDisable()
        {
            bowInteractable.selectEntered.RemoveListener(OnBowGrabbed);
            bowInteractable.selectExited.RemoveListener(OnBowReleased);
            bowInteractable.OnSecondHandGrabbed -= OnSecondHandGrabbed;
            bowInteractable.OnSecondHandReleased -= OnSecondHandReleased;
        }

        private void OnBowGrabbed(SelectEnterEventArgs args)
        {
            primaryHand = args.interactorObject as XRBaseInteractor;
        }

        private void OnBowReleased(SelectExitEventArgs args)
        {
            primaryHand = null;
            
            if (isDrawing)
            {
                CancelDraw();
            }
        }

        private void OnSecondHandGrabbed(XRBaseInteractor interactor)
        {
            StartDrawing();
        }

        private void OnSecondHandReleased(XRBaseInteractor interactor)
        {
            if (isDrawing)
            {
                ReleaseArrow();
            }
        }

        private void Update()
        {
            if (isDrawing && bowInteractable.IsSecondHandGrabbing)
            {
                UpdateDrawing();
            }
        }

        private void StartDrawing()
        {
            isDrawing = true;
            currentDrawStrength = 0f;

            // Instanziiere Pfeil
            if (arrowPrefab != null && arrowSpawnPoint != null)
            {
                GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
                currentArrow = arrowObj.GetComponent<Arrow>();
                
                if (currentArrow != null)
                {
                    currentArrow.AttachToBow(arrowSpawnPoint);
                }
            }
        }

        private void UpdateDrawing()
        {
            if (bowString == null || arrowSpawnPoint == null)
            {
                return;
            }

            // Berechne Zugstärke basierend auf der Position der zweiten Hand
            Vector3 secondHandPos = bowInteractable.GetSecondHandPosition();
            Vector3 bowPos = arrowSpawnPoint.position;
            
            // Projiziere die Hand-Position auf die Bogen-Rückwärts-Achse
            Vector3 drawDirection = -arrowSpawnPoint.forward;
            Vector3 toHand = secondHandPos - bowPos;
            float drawDistance = Vector3.Dot(toHand, drawDirection);
            
            // Normalisiere auf 0-1 Bereich (basierend auf maximaler Zugdistanz)
            float maxDrawDistance = bowString.maxDrawDistance;
            currentDrawStrength = Mathf.Clamp01(drawDistance / maxDrawDistance);

            // Aktualisiere Sehne
            bowString.UpdateDraw(currentDrawStrength, secondHandPos);

            // Haptic Feedback
            if (useHaptics && currentDrawStrength > 0.1f)
            {
                SendHapticFeedback(bowInteractable.secondHand, drawHapticIntensity * currentDrawStrength, hapticDuration);
            }
        }

        private void ReleaseArrow()
        {
            if (currentArrow == null)
            {
                CancelDraw();
                return;
            }

            // Nur schießen, wenn genug gespannt wurde
            if (currentDrawStrength >= minDrawStrength)
            {
                // Berechne Pfeilgeschwindigkeit
                float velocity = Mathf.Lerp(0, maxArrowVelocity, currentDrawStrength) * velocityMultiplier;
                Vector3 direction = arrowSpawnPoint.forward;

                // Löse Pfeil vom Bogen
                currentArrow.DetachFromBow();
                currentArrow.Launch(direction, velocity);

                // Haptic Feedback für Release
                if (useHaptics)
                {
                    SendHapticFeedback(primaryHand, releaseHapticIntensity, hapticDuration);
                    if (bowInteractable.secondHand != null)
                    {
                        SendHapticFeedback(bowInteractable.secondHand, releaseHapticIntensity, hapticDuration);
                    }
                }
            }
            else
            {
                // Pfeil zerstören, wenn nicht genug gespannt
                if (currentArrow != null)
                {
                    Destroy(currentArrow.gameObject);
                }
            }

            // Reset
            currentArrow = null;
            isDrawing = false;
            currentDrawStrength = 0f;
            bowString.ResetDraw();
        }

        private void CancelDraw()
        {
            if (currentArrow != null)
            {
                Destroy(currentArrow.gameObject);
            }

            currentArrow = null;
            isDrawing = false;
            currentDrawStrength = 0f;
            
            if (bowString != null)
            {
                bowString.ResetDraw();
            }
        }

        private void SendHapticFeedback(XRBaseInteractor interactor, float intensity, float duration)
        {
            if (interactor != null && interactor is XRBaseControllerInteractor controllerInteractor)
            {
                HapticsHelper.SendHapticImpulse(controllerInteractor, intensity, duration);
            }
        }

        // Debug Visualisierung
        private void OnDrawGizmos()
        {
            if (arrowSpawnPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(arrowSpawnPoint.position, 0.02f);
                Gizmos.DrawRay(arrowSpawnPoint.position, arrowSpawnPoint.forward * 0.3f);
            }
        }
    }
}
