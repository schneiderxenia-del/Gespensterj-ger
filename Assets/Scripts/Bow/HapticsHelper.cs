using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Helper class for sending haptic feedback to XR controllers.
    /// Provides convenient methods for common haptic patterns.
    /// </summary>
    public static class HapticsHelper
    {
        /// <summary>
        /// Send a simple haptic impulse to an interactor
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        /// <param name="amplitude">Intensity of the haptic (0-1)</param>
        /// <param name="duration">Duration in seconds</param>
        public static void SendHapticImpulse(IXRInteractor interactor, float amplitude, float duration)
        {
            if (interactor == null)
                return;

            // Get the XRBaseController from the interactor
            XRBaseController controller = GetControllerFromInteractor(interactor);
            if (controller != null)
            {
                controller.SendHapticImpulse(amplitude, duration);
            }
        }

        /// <summary>
        /// Send a pulsing haptic pattern
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        /// <param name="amplitude">Intensity of each pulse (0-1)</param>
        /// <param name="pulseCount">Number of pulses</param>
        /// <param name="pulseInterval">Time between pulses in seconds</param>
        public static void SendHapticPulse(IXRInteractor interactor, float amplitude, int pulseCount, float pulseInterval)
        {
            if (interactor == null)
                return;

            XRBaseController controller = GetControllerFromInteractor(interactor);
            if (controller == null)
                return;

            // Note: This requires a MonoBehaviour to handle coroutines
            // In practice, you would call this from a MonoBehaviour with:
            // StartCoroutine(SendHapticPulseCoroutine(controller, amplitude, pulseCount, pulseInterval));
            
            // For now, just send a single longer impulse
            float totalDuration = pulseCount * pulseInterval;
            controller.SendHapticImpulse(amplitude, totalDuration);
        }

        /// <summary>
        /// Send a haptic impulse that ramps up in intensity
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        /// <param name="maxAmplitude">Maximum intensity (0-1)</param>
        /// <param name="duration">Total duration in seconds</param>
        public static void SendHapticRampUp(IXRInteractor interactor, float maxAmplitude, float duration)
        {
            if (interactor == null)
                return;

            XRBaseController controller = GetControllerFromInteractor(interactor);
            if (controller != null)
            {
                // Simple implementation - send at max amplitude
                // A more advanced version would modulate the intensity over time
                controller.SendHapticImpulse(maxAmplitude, duration);
            }
        }

        /// <summary>
        /// Send a strong haptic burst (for impacts, releases, etc.)
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        public static void SendHapticBurst(IXRInteractor interactor)
        {
            SendHapticImpulse(interactor, 1.0f, 0.1f);
        }

        /// <summary>
        /// Send a subtle haptic tap (for UI feedback, etc.)
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        public static void SendHapticTap(IXRInteractor interactor)
        {
            SendHapticImpulse(interactor, 0.3f, 0.05f);
        }

        /// <summary>
        /// Send haptics based on a value (0-1), useful for continuous feedback
        /// </summary>
        /// <param name="interactor">The interactor to send haptics to</param>
        /// <param name="value">Normalized value (0-1) representing intensity</param>
        /// <param name="minAmplitude">Minimum haptic amplitude</param>
        /// <param name="maxAmplitude">Maximum haptic amplitude</param>
        public static void SendContinuousHaptic(IXRInteractor interactor, float value, float minAmplitude = 0.1f, float maxAmplitude = 0.8f)
        {
            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, Mathf.Clamp01(value));
            SendHapticImpulse(interactor, amplitude, Time.deltaTime);
        }

        /// <summary>
        /// Extract the XRBaseController from an interactor
        /// </summary>
        private static XRBaseController GetControllerFromInteractor(IXRInteractor interactor)
        {
            if (interactor == null)
                return null;

            // Try to get controller from the interactor's transform
            if (interactor is MonoBehaviour monoBehaviour)
            {
                XRBaseController controller = monoBehaviour.GetComponent<XRBaseController>();
                if (controller != null)
                    return controller;

                // Try parent objects
                controller = monoBehaviour.GetComponentInParent<XRBaseController>();
                if (controller != null)
                    return controller;
            }

            return null;
        }

        /// <summary>
        /// Check if an interactor supports haptics
        /// </summary>
        public static bool SupportsHaptics(IXRInteractor interactor)
        {
            return GetControllerFromInteractor(interactor) != null;
        }

        /// <summary>
        /// Stop any ongoing haptics on an interactor
        /// </summary>
        public static void StopHaptics(IXRInteractor interactor)
        {
            if (interactor == null)
                return;

            XRBaseController controller = GetControllerFromInteractor(interactor);
            if (controller != null)
            {
                // Send zero-amplitude impulse to stop
                controller.SendHapticImpulse(0f, 0f);
            }
        }
    }
}
