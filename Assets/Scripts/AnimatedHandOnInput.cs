using UnityEngine;

using UnityEngine.InputSystem;



public class AnimateAndOnInput : MonoBehaviour
{
    [Header("Optional: Automatische Anbindung für Bogen-Interaktion")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable bowGrabInteractable;

    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;

    public Animator handAnimator;

    void Awake()
    {
        // Automatische Listener-Anbindung, falls Referenz gesetzt
        if (bowGrabInteractable != null)
        {
            bowGrabInteractable.selectEntered.AddListener(_ => OnBowGrabbed());
            bowGrabInteractable.selectExited.AddListener(args => {
                if (!args.isCanceled) OnBowReleased();
            });
        }
    }

    // Beispiel: Diese Methoden können von einem XR-Grab-Event oder Collider-Trigger aufgerufen werden
    public void OnBowGrabbed()
    {
        if (handAnimator != null)
            handAnimator.SetBool("IsHoldingBow", true);
    }

    public void OnBowReleased()
    {
        if (handAnimator != null)
            handAnimator.SetBool("IsHoldingBow", false);
    }

    // Update is called once per frame
    void Update()
    {
        float trigger = triggerValue.action.ReadValue<float>();
        float grip = gripValue.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);
        // Der Parameter "IsHoldingBow" wird über OnBowGrabbed/OnBowReleased gesetzt
    }

    // Beispiel für die Anbindung (z.B. im XR Grab Interactable):
    // bowGrabInteractable.onSelectEntered.AddListener(handScript.OnBowGrabbed);
    // bowGrabInteractable.onSelectExited.AddListener(handScript.OnBowReleased);
}
