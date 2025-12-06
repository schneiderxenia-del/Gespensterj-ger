using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GhostHunter.Bow
{
    /// <summary>
    /// Hilfsklasse für Haptic Feedback in XR.
    /// Bietet vereinfachte Methoden zum Senden von haptischen Impulsen an XR-Controller.
    /// </summary>
    public static class HapticsHelper
    {
        /// <summary>
        /// Sendet einen haptischen Impuls an einen XR-Controller
        /// </summary>
        /// <param name="interactor">Der XR-Interactor (Controller)</param>
        /// <param name="intensity">Intensität des Impulses (0-1)</param>
        /// <param name="duration">Dauer des Impulses in Sekunden</param>
        public static void SendHapticImpulse(XRBaseControllerInteractor interactor, float intensity, float duration)
        {
            if (interactor == null)
            {
                return;
            }

            // Versuche, das XRController-Interface zu verwenden
            if (interactor.xrController != null)
            {
                interactor.xrController.SendHapticImpulse(Mathf.Clamp01(intensity), duration);
            }
        }

        /// <summary>
        /// Sendet einen haptischen Impuls an beide Controller
        /// </summary>
        /// <param name="leftInteractor">Linker Controller</param>
        /// <param name="rightInteractor">Rechter Controller</param>
        /// <param name="intensity">Intensität des Impulses (0-1)</param>
        /// <param name="duration">Dauer des Impulses in Sekunden</param>
        public static void SendHapticImpulseToBoth(XRBaseControllerInteractor leftInteractor, 
                                                    XRBaseControllerInteractor rightInteractor, 
                                                    float intensity, float duration)
        {
            SendHapticImpulse(leftInteractor, intensity, duration);
            SendHapticImpulse(rightInteractor, intensity, duration);
        }

        /// <summary>
        /// Sendet einen schwachen haptischen Impuls (für subtiles Feedback)
        /// </summary>
        public static void SendLightHaptic(XRBaseControllerInteractor interactor)
        {
            SendHapticImpulse(interactor, 0.1f, 0.05f);
        }

        /// <summary>
        /// Sendet einen mittleren haptischen Impuls (für normales Feedback)
        /// </summary>
        public static void SendMediumHaptic(XRBaseControllerInteractor interactor)
        {
            SendHapticImpulse(interactor, 0.3f, 0.1f);
        }

        /// <summary>
        /// Sendet einen starken haptischen Impuls (für starkes Feedback)
        /// </summary>
        public static void SendStrongHaptic(XRBaseControllerInteractor interactor)
        {
            SendHapticImpulse(interactor, 0.8f, 0.2f);
        }

        /// <summary>
        /// Sendet ein "Click"-Feedback
        /// </summary>
        public static void SendClickFeedback(XRBaseControllerInteractor interactor)
        {
            SendHapticImpulse(interactor, 0.5f, 0.05f);
        }

        /// <summary>
        /// Sendet ein "Impact"-Feedback (kurz und stark)
        /// </summary>
        public static void SendImpactFeedback(XRBaseControllerInteractor interactor)
        {
            SendHapticImpulse(interactor, 1.0f, 0.1f);
        }

        /// <summary>
        /// Sendet eine Haptic-Sequenz (mehrere Impulse nacheinander)
        /// </summary>
        public static void SendHapticSequence(MonoBehaviour context, XRBaseControllerInteractor interactor, 
                                              float[] intensities, float[] durations, float[] delays)
        {
            if (context == null || interactor == null || intensities == null || durations == null || delays == null)
            {
                return;
            }

            if (intensities.Length != durations.Length || intensities.Length != delays.Length)
            {
                Debug.LogWarning("HapticsHelper: Array lengths for sequence must match!");
                return;
            }

            context.StartCoroutine(HapticSequenceCoroutine(interactor, intensities, durations, delays));
        }

        private static System.Collections.IEnumerator HapticSequenceCoroutine(XRBaseControllerInteractor interactor,
                                                                               float[] intensities, 
                                                                               float[] durations, 
                                                                               float[] delays)
        {
            for (int i = 0; i < intensities.Length; i++)
            {
                SendHapticImpulse(interactor, intensities[i], durations[i]);
                yield return new WaitForSeconds(delays[i]);
            }
        }
    }
}
