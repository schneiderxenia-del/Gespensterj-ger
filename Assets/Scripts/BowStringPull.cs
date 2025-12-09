using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    // Steuerung des Spannens der Bogensehne
    [HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/BowStringPull")]
    public class BowStringPull : XRBaseInteractable
    {
        // Ereignisse für andere Systeme (z. B. Pfeilabschuss)
        public event Action<float> OnReleased;      // Beim Loslassen der Sehne
        public event Action<float> OnPullChanged;   // Während der Sehne gezogen wird
        public event Action OnPullStart;            // Beginn des Ziehvorgangs
        public event Action OnPullEnd;              // Zurücksetzen der Sehne

        [Header("EInstellungen für Bogensehne")]
        [SerializeField] private Transform startPos;    // Startpunkt der Sehne
        [SerializeField] private Transform endPos;      // Maximaler Zugpunkt
        [SerializeField] private GameObject stringPos;  // Sichtbare Sehnenposition

        public float pullLevel { get; private set; } = 0f; // Zugstärke (0–1)

        private LineRenderer bowString;                // Darstellung der Sehne
        private IXRSelectInteractor activeHand = null; // Hand, die zieht

        protected override void Awake()
        {
            base.Awake();
            bowString = GetComponent<LineRenderer>();
        }

        // Wird aufgerufen, wenn der Spieler die Sehne greift
        public void BeginPull(SelectEnterEventArgs args)
        {
            activeHand = args.interactorObject;
            OnPullStart?.Invoke();
        }

        // Wird ausgeführt, wenn der Spieler loslässt
        public void EndPull()
        {
            OnReleased?.Invoke(pullLevel);
            OnPullEnd?.Invoke();

            activeHand = null;
            pullLevel = 0f;

            // Sehne visuell zurücksetzen
            stringPos.transform.localPosition =
                new Vector3(stringPos.transform.localPosition.x, stringPos.transform.localPosition.y, 0f);

            UpdateBowString();
        }

        // Hauptlogik – wird pro Frame aufgerufen
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // Nur wenn Sehne gehalten wird
                if (isSelected && activeHand != null)
                {
                    Vector3 handPos = activeHand.GetAttachTransform(this).position;
                    float oldValue = pullLevel;

                    // Zugstärke berechnen
                    pullLevel = ComputePullLevel(handPos);

                    // Event nur senden, wenn sich der Wert ändert
                    if (oldValue != pullLevel)
                        OnPullChanged?.Invoke(pullLevel);

                    UpdateBowString();
                    SendHaptics();
                }
            }
        }

        // Startet den Ziehvorgang
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            BeginPull(args);
        }

        // Berechnet die Zugstärke zwischen 0 und 1
        private float ComputePullLevel(Vector3 handPos)
        {
            Vector3 pullDir = handPos - startPos.position;
            Vector3 targetDir = endPos.position - startPos.position;

            float maxDist = targetDir.magnitude;

            targetDir.Normalize();
            float value = Vector3.Dot(pullDir, targetDir) / maxDist;

            return Mathf.Clamp(value, 0f, 1f);
        }

        // Aktualisiert die sichtbare Position der Sehne
        private void UpdateBowString()
        {
            Vector3 pos =
                Vector3.Lerp(startPos.localPosition, endPos.localPosition, pullLevel);

            stringPos.transform.localPosition = pos;
            bowString.SetPosition(1, pos);
        }

        // Vibrationsfeedback für VR-Controller
        private void SendHaptics()
        {
            if (activeHand is XRBaseInputInteractor controller)
                controller.SendHapticImpulse(pullLevel, 0.1f);
        }
    }
}
