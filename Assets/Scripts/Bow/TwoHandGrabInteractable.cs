using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Base class for interactables that can be grabbed with two hands.
    /// Extends XRGrabInteractable to support a second hand attachment point.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TwoHandGrabInteractable : XRGrabInteractable
    {
        [Header("Two Hand Settings")]
        [SerializeField]
        [Tooltip("The secondary attachment point for the second hand")]
        private Transform secondaryAttachTransform;

        [SerializeField]
        [Tooltip("Minimum distance between hands to consider it a valid two-hand grab")]
        private float minHandDistance = 0.1f;

        [SerializeField]
        [Tooltip("Maximum distance between hands for two-hand interaction")]
        private float maxHandDistance = 1.5f;

        protected IXRSelectInteractor secondHandInteractor;
        protected bool isTwoHandGrabbed = false;

        public Transform SecondaryAttachTransform => secondaryAttachTransform;
        public IXRSelectInteractor SecondHandInteractor => secondHandInteractor;
        public bool IsTwoHandGrabbed => isTwoHandGrabbed;

        protected override void Awake()
        {
            base.Awake();

            // Ensure we have a secondary attach transform
            if (secondaryAttachTransform == null)
            {
                GameObject secondaryAttach = new GameObject("SecondaryAttach");
                secondaryAttach.transform.SetParent(transform);
                secondaryAttach.transform.localPosition = Vector3.forward * 0.3f;
                secondaryAttach.transform.localRotation = Quaternion.identity;
                secondaryAttachTransform = secondaryAttach.transform;
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            CheckForTwoHandGrab(args.interactorObject);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            if (args.interactorObject == secondHandInteractor)
            {
                OnSecondHandGrabEnd();
            }

            base.OnSelectExited(args);
        }

        /// <summary>
        /// Check if a new interactor should become the second hand
        /// </summary>
        protected virtual void CheckForTwoHandGrab(IXRSelectInteractor interactor)
        {
            // If we already have a primary hand and this is a different interactor
            if (isSelected && interactor != firstInteractorSelecting && secondHandInteractor == null)
            {
                float distance = Vector3.Distance(
                    GetInteractorPosition(firstInteractorSelecting),
                    GetInteractorPosition(interactor)
                );

                if (distance >= minHandDistance && distance <= maxHandDistance)
                {
                    secondHandInteractor = interactor;
                    isTwoHandGrabbed = true;
                    OnSecondHandGrabStart();
                }
            }
        }

        /// <summary>
        /// Called when the second hand starts grabbing
        /// </summary>
        protected virtual void OnSecondHandGrabStart()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called when the second hand stops grabbing
        /// </summary>
        protected virtual void OnSecondHandGrabEnd()
        {
            secondHandInteractor = null;
            isTwoHandGrabbed = false;
        }

        /// <summary>
        /// Get the world position of an interactor
        /// </summary>
        protected Vector3 GetInteractorPosition(IXRInteractor interactor)
        {
            if (interactor is XRBaseInteractor baseInteractor)
            {
                return baseInteractor.attachTransform.position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Get the distance between the two hands when two-hand grabbed
        /// </summary>
        public float GetHandsDistance()
        {
            if (!isTwoHandGrabbed || secondHandInteractor == null)
                return 0f;

            return Vector3.Distance(
                GetInteractorPosition(firstInteractorSelecting),
                GetInteractorPosition(secondHandInteractor)
            );
        }

        /// <summary>
        /// Get the position of the second hand in world space
        /// </summary>
        public Vector3 GetSecondHandPosition()
        {
            if (secondHandInteractor != null)
            {
                return GetInteractorPosition(secondHandInteractor);
            }
            return secondaryAttachTransform.position;
        }

        /// <summary>
        /// Get the normalized pull value (0 to 1) based on hand distance
        /// </summary>
        public float GetPullValue(float maxPullDistance)
        {
            if (!isTwoHandGrabbed)
                return 0f;

            float currentDistance = GetHandsDistance();
            return Mathf.Clamp01(currentDistance / maxPullDistance);
        }
    }
}
