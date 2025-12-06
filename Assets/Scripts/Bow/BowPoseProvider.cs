using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Bietet Hand-Poses für das Halten des Bogens.
    /// Kann erweitert werden, um mit XR Hand Tracking zu arbeiten.
    /// </summary>
    public class BowPoseProvider : MonoBehaviour
    {
        [Header("Pose Settings")]
        [Tooltip("Pose für die primäre Hand (hält den Bogen)")]
        [SerializeField] private HandPose primaryHandPose;

        [Tooltip("Pose für die sekundäre Hand (zieht die Sehne)")]
        [SerializeField] private HandPose secondaryHandPose;

        [Header("Hand Offsets")]
        [Tooltip("Positions-Offset für die primäre Hand")]
        [SerializeField] private Vector3 primaryHandPositionOffset = Vector3.zero;

        [Tooltip("Rotations-Offset für die primäre Hand")]
        [SerializeField] private Vector3 primaryHandRotationOffset = Vector3.zero;

        [Tooltip("Positions-Offset für die sekundäre Hand")]
        [SerializeField] private Vector3 secondaryHandPositionOffset = Vector3.zero;

        [Tooltip("Rotations-Offset für die sekundäre Hand")]
        [SerializeField] private Vector3 secondaryHandRotationOffset = Vector3.zero;

        private TwoHandGrabInteractable bowInteractable;

        private void Awake()
        {
            bowInteractable = GetComponent<TwoHandGrabInteractable>();
        }

        private void OnEnable()
        {
            if (bowInteractable != null)
            {
                bowInteractable.selectEntered.AddListener(OnBowGrabbed);
                bowInteractable.OnSecondHandGrabbed += OnSecondHandGrabbed;
            }
        }

        private void OnDisable()
        {
            if (bowInteractable != null)
            {
                bowInteractable.selectEntered.RemoveListener(OnBowGrabbed);
                bowInteractable.OnSecondHandGrabbed -= OnSecondHandGrabbed;
            }
        }

        private void OnBowGrabbed(SelectEnterEventArgs args)
        {
            ApplyPoseToHand(args.interactorObject as XRBaseInteractor, primaryHandPose, 
                           primaryHandPositionOffset, primaryHandRotationOffset);
        }

        private void OnSecondHandGrabbed(XRBaseInteractor interactor)
        {
            ApplyPoseToHand(interactor, secondaryHandPose, 
                           secondaryHandPositionOffset, secondaryHandRotationOffset);
        }

        private void ApplyPoseToHand(XRBaseInteractor interactor, HandPose pose, 
                                     Vector3 positionOffset, Vector3 rotationOffset)
        {
            if (interactor == null || pose == null)
            {
                return;
            }

            // Hier könnte die Pose auf die Hand angewendet werden
            // Dies hängt vom verwendeten Hand-Tracking-System ab
            // Für XR Hands oder andere Hand-Tracking-Lösungen müsste hier
            // die entsprechende Implementierung erfolgen

            Debug.Log($"Applying bow pose to {interactor.name}");
        }

        /// <summary>
        /// Gibt die Attach-Position für die primäre Hand zurück
        /// </summary>
        public Vector3 GetPrimaryHandAttachPosition()
        {
            return transform.position + transform.TransformDirection(primaryHandPositionOffset);
        }

        /// <summary>
        /// Gibt die Attach-Rotation für die primäre Hand zurück
        /// </summary>
        public Quaternion GetPrimaryHandAttachRotation()
        {
            return transform.rotation * Quaternion.Euler(primaryHandRotationOffset);
        }

        private void OnDrawGizmos()
        {
            // Visualisiere Hand-Attach-Punkte
            Gizmos.color = Color.blue;
            Vector3 primaryPos = transform.position + transform.TransformDirection(primaryHandPositionOffset);
            Gizmos.DrawWireSphere(primaryPos, 0.03f);
            Gizmos.DrawLine(primaryPos, primaryPos + transform.forward * 0.1f);

            Gizmos.color = Color.green;
            Vector3 secondaryPos = transform.position + transform.TransformDirection(secondaryHandPositionOffset);
            Gizmos.DrawWireSphere(secondaryPos, 0.03f);
        }
    }

    /// <summary>
    /// Definiert eine Hand-Pose
    /// </summary>
    [System.Serializable]
    public class HandPose
    {
        [Tooltip("Name der Pose")]
        public string poseName = "Default";

        [Tooltip("Finger-Curl-Werte (0 = offen, 1 = geschlossen)")]
        [Range(0f, 1f)] public float thumbCurl = 0f;
        [Range(0f, 1f)] public float indexCurl = 0f;
        [Range(0f, 1f)] public float middleCurl = 0f;
        [Range(0f, 1f)] public float ringCurl = 0f;
        [Range(0f, 1f)] public float pinkyCurl = 0f;

        [Tooltip("Handgelenk-Rotation")]
        public Vector3 wristRotation = Vector3.zero;

        public HandPose()
        {
        }

        public HandPose(string name)
        {
            poseName = name;
        }

        /// <summary>
        /// Erstellt eine Standard-Greif-Pose
        /// </summary>
        public static HandPose CreateGripPose()
        {
            return new HandPose("Grip")
            {
                thumbCurl = 0.7f,
                indexCurl = 0.8f,
                middleCurl = 0.9f,
                ringCurl = 1f,
                pinkyCurl = 1f
            };
        }

        /// <summary>
        /// Erstellt eine Pinch-Pose (für das Ziehen der Sehne)
        /// </summary>
        public static HandPose CreatePinchPose()
        {
            return new HandPose("Pinch")
            {
                thumbCurl = 0.5f,
                indexCurl = 0.6f,
                middleCurl = 0.6f,
                ringCurl = 0.3f,
                pinkyCurl = 0.2f
            };
        }
    }
}
