using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Erweitert XRGrabInteractable für zweihändige Interaktion.
    /// Ermöglicht es, dass ein Objekt mit beiden Händen gegriffen werden kann.
    /// </summary>
    public class TwoHandGrabInteractable : XRGrabInteractable
    {
        [Header("Two Hand Settings")]
        [Tooltip("Die zweite Hand, die das Objekt greift")]
        public XRBaseInteractor secondHand;

        [Tooltip("Das Transform der zweiten Hand Attach-Position")]
        public Transform secondHandAttachTransform;

        public event Action<XRBaseInteractor> OnSecondHandGrabbed;
        public event Action<XRBaseInteractor> OnSecondHandReleased;

        private bool isSecondHandGrabbing = false;

        /// <summary>
        /// Prüft, ob eine zweite Hand das Objekt greift
        /// </summary>
        public bool IsSecondHandGrabbing => isSecondHandGrabbing;

        /// <summary>
        /// Versucht, das Objekt mit einer zweiten Hand zu greifen
        /// </summary>
        public virtual bool TryGrabWithSecondHand(XRBaseInteractor interactor)
        {
            if (isSecondHandGrabbing || !isSelected)
            {
                return false;
            }

            secondHand = interactor;
            isSecondHandGrabbing = true;
            OnSecondHandGrabbed?.Invoke(interactor);
            return true;
        }

        /// <summary>
        /// Lässt die zweite Hand los
        /// </summary>
        public virtual void ReleaseSecondHand()
        {
            if (!isSecondHandGrabbing)
            {
                return;
            }

            var releasedHand = secondHand;
            secondHand = null;
            isSecondHandGrabbing = false;
            OnSecondHandReleased?.Invoke(releasedHand);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // Wenn die primäre Hand loslässt, release auch die zweite Hand
            if (isSecondHandGrabbing)
            {
                ReleaseSecondHand();
            }
        }

        /// <summary>
        /// Gibt die Position der zweiten Hand zurück, falls vorhanden
        /// </summary>
        public Vector3 GetSecondHandPosition()
        {
            if (isSecondHandGrabbing && secondHand != null)
            {
                return secondHand.transform.position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Gibt die Rotation der zweiten Hand zurück, falls vorhanden
        /// </summary>
        public Quaternion GetSecondHandRotation()
        {
            if (isSecondHandGrabbing && secondHand != null)
            {
                return secondHand.transform.rotation;
            }
            return Quaternion.identity;
        }
    }
}
